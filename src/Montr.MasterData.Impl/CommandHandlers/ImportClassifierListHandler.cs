using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	// todo: move to service? should recieve stream?
	public class ImportClassifierListHandler : IRequestHandler<ImportClassifierList, ImportResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		private ClassifierType _type;
		private DbClassifierTree _tree;

		private IDictionary<string, DbClassifier> _existingItems;
		private IDictionary<string, DbClassifierGroup> _existingGroups;

		public ImportClassifierListHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<ImportResult> Handle(ImportClassifierList request, CancellationToken cancellationToken)
		{
			// todo: write with unit test for each todo
			// (+) todo: build DAG for groups and items
			// (+) todo: build dictionaries
			// todo: generate codes for new entities
			// (+) todo: add new items to default tree
			// (+) todo: build tree of items
			// (+) todo: build closure table for groups or item

			await Preload(request, cancellationToken);

			ImportResult result;

			if (_existingItems.IsNullOrEmpty() && _existingGroups.IsNullOrEmpty())
			{
				result = BulkImport(request);
			}
			else
			{
				result = await Import(request, cancellationToken);
			}

			return result;
		}

		/// <summary>
		/// Preload all existing data from db - type, items.
		/// For Groups hierarchy preload tree (create if not exists), groups.
		/// </summary>
		private async Task Preload(ImportClassifierList request, CancellationToken cancellationToken)
		{
			_type = await _classifierTypeService.GetClassifierType(request.TypeCode, cancellationToken);

			using (var db = _dbContextFactory.Create())
			{
				_existingItems = await db.GetTable<DbClassifier>()
					.Where(x => x.TypeUid == _type.Uid)
					.ToDictionaryAsync(x => x.Code, cancellationToken);

				if (_type.HierarchyType == HierarchyType.Groups)
				{
					_tree = db.GetTable<DbClassifierTree>()
						.SingleOrDefault(x =>
							x.TypeUid == _type.Uid &&
							x.Code == ClassifierTree.DefaultCode);

					if (_tree == null)
					{
						_tree = new DbClassifierTree
						{
							Uid = Guid.NewGuid(),
							TypeUid = _type.Uid,
							Code = ClassifierTree.DefaultCode,
							Name = _type.Name
						};

						await db.GetTable<DbClassifierTree>()
							.Value(x => x.Uid, _tree.Uid)
							.Value(x => x.TypeUid, _tree.TypeUid)
							.Value(x => x.Code, _tree.Code)
							.Value(x => x.Name, _tree.Name)
							.InsertAsync(cancellationToken);
					}

					_existingGroups = await db.GetTable<DbClassifierGroup>()
						.Where(x => x.TreeUid == _tree.Uid)
						.ToDictionaryAsync(x => x.Code, cancellationToken);
				}
			}
		}

		private ImportResult BulkImport(ImportClassifierList request)
		{
			var activeItems = new Dictionary<string, DbClassifier>();
			var inactiveItems = new Dictionary<string, DbClassifier>();
			var groups = new Dictionary<string, DbClassifierGroup>();
			var links = new List<DbClassifierLink>();
			var closures = new ClosureMap();

			if (request.Data.Items != null)
			{
				var sortedItems = DirectedAcyclicGraphVerifier.TopologicalSort(
					request.Data.Items,
					node => node.Code,
					node => node.ParentCode != null ? new[] { node.ParentCode } : null);

				foreach (var item in sortedItems)
				{
					var itemUid = Guid.NewGuid();

					Guid? parentUid = null;

					if (_type.HierarchyType == HierarchyType.Items)
					{
						if (item.ParentCode != null)
						{
							if (activeItems.TryGetValue(item.ParentCode, out var parent))
							{
								parentUid = parent.Uid;
							}
							else if (inactiveItems.TryGetValue(item.ParentCode, out parent))
							{
								parentUid = parent.Uid;
							}
						}

						closures.Insert(itemUid, parentUid);
					}

					var items =
						item.StatusCode == ClassifierStatusCode.Active
							? activeItems
							: inactiveItems;

					items[item.Code] = new DbClassifier
					{
						Uid = itemUid,
						TypeUid = _type.Uid,
						StatusCode = item.StatusCode,
						Code = item.Code,
						Name = item.Name,
						ParentUid = parentUid
					};
				}
			}

			if (_type.HierarchyType == HierarchyType.Groups)
			{
				if (request.Data.Groups != null)
				{
					var sortedGroups = DirectedAcyclicGraphVerifier.TopologicalSort(
						request.Data.Groups,
						node => node.Code,
						node => node.ParentCode != null ? new[] { node.ParentCode } : null);

					foreach (var group in sortedGroups)
					{
						var groupUid = Guid.NewGuid();

						Guid? parentUid = null;

						if (group.ParentCode != null)
						{
							parentUid = groups[group.ParentCode].Uid;
						}

						groups[group.Code] = new DbClassifierGroup
						{
							Uid = groupUid,
							TreeUid = _tree.Uid,
							Code = group.Code,
							Name = group.Name,
							ParentUid = parentUid
						};

						closures.Insert(groupUid, parentUid);
					}
				}

				// todo: link all unlinked items to root
				if (request.Data.Links != null)
				{
					foreach (var link in request.Data.Links)
					{
						var items =
							link.ItemStatusCode == ClassifierStatusCode.Active
							? activeItems : inactiveItems;

						links.Add(new DbClassifierLink
						{
							GroupUid = groups[link.GroupCode].Uid,
							ItemUid = items[link.ItemCode].Uid,
						});
					}
				}
			}

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var copyOptions = new BulkCopyOptions { BulkCopyType = BulkCopyType.ProviderSpecific };

					// todo: move heavy operations outside of open connection

					if (activeItems.Values.Count > 0 || inactiveItems.Values.Count > 0)
					{
						var sorted = DirectedAcyclicGraphVerifier.TopologicalSort(
							activeItems.Values.Union(inactiveItems.Values).ToList(),
							node => node.Uid,
							node => node.ParentUid != null ? new[] { node.ParentUid } : null);

						db.BulkCopy(copyOptions, sorted);
					}

					if (groups.Values.Count > 0)
					{
						var sorted = DirectedAcyclicGraphVerifier.TopologicalSort(
							groups.Values,
							node => node.Uid,
							node => node.ParentUid != null ? new[] { node.ParentUid } : null);

						db.BulkCopy(copyOptions, sorted);
					}

					if (links.Count > 0)
					{
						db.BulkCopy(copyOptions, links);
					}

					db.BulkCopy(copyOptions, closures.GetAll());
				}

				scope.Commit();
			}

			return new ImportResult();
		}

		private async Task<ImportResult> Import(ImportClassifierList request, CancellationToken cancellationToken)
		{
			var errors = new List<string>();

			var sortedItems = request.Data.Items != null
				? DirectedAcyclicGraphVerifier.TopologicalSort(request.Data.Items, node => node.Code,
					node => node.ParentCode != null ? new[] { node.ParentCode } : null) : null;

			var sortedGroups = _type.HierarchyType == HierarchyType.Groups && request.Data.Groups != null
				? DirectedAcyclicGraphVerifier.TopologicalSort(request.Data.Groups,
					node => node.Code, node => node.ParentCode != null ? new[] { node.ParentCode } : null) : null;

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var closureTable = new ClosureTableHandler(db, _type);

					if (sortedItems != null)
					{
						foreach (var item in sortedItems)
						{
							if (_existingItems.TryGetValue(item.Code, out var dbItem) == false)
							{
								var itemUid = Guid.NewGuid();

								Guid? parentUid = null;

								if (_type.HierarchyType == HierarchyType.Items && item.ParentCode != null)
								{
									if (_existingItems.TryGetValue(item.ParentCode, out var dbParentItem))
									{
										parentUid = dbParentItem.Uid;
									}
									else
									{
										errors.Add($"Item {item.ParentCode} specified as parent for item {item.Code} not found in classifier {_type.Code}.");
									}
								}

								await db.GetTable<DbClassifier>()
									.Value(x => x.Uid, itemUid)
									.Value(x => x.TypeUid, _type.Uid)
									.Value(x => x.StatusCode, item.StatusCode)
									.Value(x => x.Code, item.Code)
									.Value(x => x.Name, item.Name)
									.Value(x => x.ParentUid, parentUid)
									.InsertAsync(cancellationToken);

								// todo: check result and throw error
								await closureTable.Insert(itemUid, parentUid, cancellationToken);

								_existingItems.Add(item.Code, new DbClassifier
								{
									Uid = itemUid,
									TypeUid = _type.Uid,
									StatusCode = item.StatusCode,
									Code = item.Code,
									Name = item.Name,
									ParentUid = parentUid
								});
							}
							else
							{
								var stmt = db.GetTable<DbClassifier>()
									.Where(x => x.Uid == dbItem.Uid)
									.AsUpdatable();

								var changed = false;

								if (dbItem.Name != item.Name)
								{
									stmt = stmt.Set(x => x.Name, item.Name);
									changed = true;
								}

								if (dbItem.StatusCode != item.StatusCode)
								{
									stmt = stmt.Set(x => x.StatusCode, item.StatusCode);
									changed = true;
								}

								if (changed)
								{
									await stmt.UpdateAsync(cancellationToken);
								}
							}
						}
					}

					if (_type.HierarchyType == HierarchyType.Groups)
					{
						if (sortedGroups != null)
						{
							foreach (var group in sortedGroups)
							{
								if (_existingGroups.TryGetValue(group.Code, out var dbGroup) == false)
								{
									Guid? parentUid = null;

									if (group.ParentCode != null)
									{
										if (_existingGroups.TryGetValue(group.ParentCode, out var parentGroup))
										{
											parentUid = parentGroup.Uid;
										}
										else
										{
											// todo: validate all before import or throw error 
											errors.Add($"Group {group.ParentCode} specified as parent for group {group.Code} not found in classifier {_type.Code}.");
										}
									}

									var groupUid = Guid.NewGuid();

									await db.GetTable<DbClassifierGroup>()
										.Value(x => x.Uid, groupUid)
										.Value(x => x.TreeUid, _tree.Uid)
										.Value(x => x.Code, group.Code)
										.Value(x => x.Name, group.Name)
										.Value(x => x.ParentUid, parentUid)
										.InsertAsync(cancellationToken);

									// todo: check result and throw error
									await closureTable.Insert(groupUid, parentUid, cancellationToken);

									_existingGroups.Add(group.Code, new DbClassifierGroup
									{
										Uid = groupUid,
										TreeUid = _tree.Uid,
										Code = group.Code,
										Name = group.Name,
										ParentUid = parentUid
									});
								}
								else
								{
									var stmt = db.GetTable<DbClassifierGroup>()
										.Where(x => x.Uid == dbGroup.Uid)
										.AsUpdatable();

									var changed = false;

									if (dbGroup.Name != group.Name)
									{
										stmt = stmt.Set(x => x.Name, group.Name);
										changed = true;
									}

									if (changed)
									{
										await stmt.UpdateAsync(cancellationToken);
									}
								}
							}
						}

						if (request.Data.Links != null)
						{
							var existingLinks = new HashSet<Tuple<Guid, Guid>>(
								(from link in db.GetTable<DbClassifierLink>()
									join g in db.GetTable<DbClassifierGroup>() on link.GroupUid equals g.Uid
									join i in db.GetTable<DbClassifier>() on link.ItemUid equals i.Uid
									where i.TypeUid == _type.Uid && g.TreeUid == _tree.Uid
									select new { link.GroupUid, link.ItemUid })
								.Select(x => Tuple.Create(x.GroupUid, x.ItemUid)));

							foreach (var itemInGroup in request.Data.Links)
							{
								if (_existingGroups.TryGetValue(itemInGroup.GroupCode, out var dbGroup) == false)
								{
									errors.Add($"Group {itemInGroup.GroupCode} specified in link for item {itemInGroup.ItemCode} not found in classifier {_type.Code}.");
								}

								if (_existingItems.TryGetValue(itemInGroup.ItemCode, out var dbItem) == false)
								{
									errors.Add($"Item {itemInGroup.ItemCode} specified in link for group {itemInGroup.GroupCode} not found in classifier {_type.Code}.");
								}

								var link = Tuple.Create(dbGroup.Uid, dbItem.Uid);

								if (existingLinks.Contains(link) == false)
								{
									await db.GetTable<DbClassifierLink>()
										.Value(x => x.GroupUid, dbGroup.Uid)
										.Value(x => x.ItemUid, dbItem.Uid)
										.InsertAsync(cancellationToken);

									existingLinks.Add(link);
								}
							}
						}
					}
				}

				scope.Commit();
			}

			return new ImportResult { Errors = errors };
		}

		private class ClosureMap
		{
			readonly IDictionary<Guid, IList<DbClassifierClosure>> _closuresByChildUid = new Dictionary<Guid, IList<DbClassifierClosure>>();

			public IEnumerable<DbClassifierClosure> GetAll()
			{
				return _closuresByChildUid.SelectMany(x => x.Value);
			}

			public long Insert(Guid itemUid, Guid? parentUid)
			{
				// insert self closure with level 0
				var itemClosures = new List<DbClassifierClosure>
				{
					new DbClassifierClosure
					{
						ParentUid = itemUid,
						ChildUid = itemUid,
						Level = 0
					}
				};

				if (parentUid.HasValue)
				{
					// insert parent closures with level + 1
					// todo: throw error if parent closures not found?
					if (_closuresByChildUid.TryGetValue(parentUid.Value, out var parentClosures))
					{
						itemClosures.AddRange(parentClosures
							.Select(x => new DbClassifierClosure
							{
								ParentUid = x.ParentUid,
								ChildUid = itemUid,
								Level = (short) (x.Level + 1)
							}));
					}
				}

				_closuresByChildUid.Add(itemUid, itemClosures);

				return itemClosures.Count;
			}
		}
	}
}
