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
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	// todo: move to service? should recieve stream?
	public class ImportClassifierListHandler : IRequestHandler<ImportClassifierList, Guid>
	{
		private const string DefaultTreeCode = "default";

		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		public ImportClassifierListHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<Guid> Handle(ImportClassifierList request, CancellationToken cancellationToken)
		{
			// todo: start with unit test for each todo
			// (+) todo: build DAG for groups and items
			// todo: build dictionaries
			// todo: generate codes for new entities
			// (+) todo: add new items to default tree
			// (+) todo: build tree of items
			// (+) todo: build closure table for groups or item

			var errors = new List<string>();

			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, request.TypeCode, cancellationToken);

			var sortedItems = request.Data.Items != null
				? DirectedAcyclicGraphVerifier.TopologicalSort(request.Data.Items,
					node => node.Code, node => node.ParentCode != null ? new [] { node.ParentCode } : null) : null;

			var sortedGroups = (type.HierarchyType == HierarchyType.Groups && request.Data.Groups != null)
				? DirectedAcyclicGraphVerifier.TopologicalSort(request.Data.Groups,
					node => node.Code, node => node.ParentCode != null ? new [] { node.ParentCode } : null) : null;

			// using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var existingItems = await db.GetTable<DbClassifier>()
						.Where(x => x.TypeUid == type.Uid)
						.ToDictionaryAsync(x => x.Code, cancellationToken);

					var closure = new ClosureTable(db);

					if (sortedItems != null)
					{
						foreach (var item in sortedItems)
						{
							if (existingItems.TryGetValue(item.Code, out var dbItem) == false)
							{
								var itemUid = Guid.NewGuid();

								Guid? parentUid = null;

								if (type.HierarchyType == HierarchyType.Items && item.ParentCode != null)
								{
									if (existingItems.TryGetValue(item.ParentCode, out var dbParentItem))
									{
										parentUid = dbParentItem.Uid;
									}
									else
									{
										errors.Add($"Item {item.ParentCode} specified as parent for item {item.Code} not found in classifier {type.Code}.");
									}
								}

								await db.GetTable<DbClassifier>()
									.Value(x => x.Uid, itemUid)
									.Value(x => x.TypeUid, type.Uid)
									.Value(x => x.StatusCode, item.StatusCode)
									.Value(x => x.Code, item.Code)
									.Value(x => x.Name, item.Name)
									.Value(x => x.ParentUid, parentUid)
									.InsertAsync(cancellationToken);

								await closure.Insert(itemUid, parentUid, cancellationToken);

								existingItems.Add(item.Code, new DbClassifier
								{
									Uid = itemUid,
									TypeUid = type.Uid,
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

					if (type.HierarchyType == HierarchyType.Groups)
					{
						Guid treeUid;

						var tree = db.GetTable<DbClassifierTree>()
							.SingleOrDefault(x =>
								x.TypeUid == type.Uid &&
								x.Code == DefaultTreeCode);

						if (tree == null)
						{
							await db.GetTable<DbClassifierTree>()
								.Value(x => x.Uid, treeUid = Guid.NewGuid())
								.Value(x => x.TypeUid, type.Uid)
								.Value(x => x.Code, DefaultTreeCode)
								.Value(x => x.Name, type.Name)
								.InsertAsync(cancellationToken);
						}
						else
						{
							treeUid = tree.Uid;
						}

						var existingGroups = await db.GetTable<DbClassifierGroup>()
							.Where(x => x.TreeUid == treeUid)
							.ToDictionaryAsync(x => x.Code, cancellationToken);

						if (sortedGroups != null)
						{
							foreach (var group in sortedGroups)
							{
								if (existingGroups.TryGetValue(group.Code, out var dbGroup) == false)
								{
									Guid? parentUid = null;

									if (group.ParentCode != null)
									{
										if (existingGroups.TryGetValue(group.ParentCode, out var parentGroup))
										{
											parentUid = parentGroup.Uid;
										}
										else
										{
											errors.Add($"Group {group.ParentCode} specified as parent for group {group.Code} not found in classifier {type.Code}.");
										}
									}

									var groupUid = Guid.NewGuid();

									await db.GetTable<DbClassifierGroup>()
										.Value(x => x.Uid, groupUid)
										.Value(x => x.TreeUid, treeUid)
										.Value(x => x.Code, group.Code)
										.Value(x => x.Name, group.Name)
										.Value(x => x.ParentUid, parentUid)
										.InsertAsync(cancellationToken);

									await closure.Insert(groupUid, parentUid, cancellationToken);

									existingGroups.Add(group.Code, new DbClassifierGroup
									{
										Uid = groupUid,
										TreeUid = treeUid,
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
									where i.TypeUid == type.Uid && g.TreeUid == treeUid
									select new {link.GroupUid, link.ItemUid })
								.Select(x => Tuple.Create(x.GroupUid, x.ItemUid)));

							foreach (var itemInGroup in request.Data.Links)
							{
								if (existingGroups.TryGetValue(itemInGroup.GroupCode, out var dbGroup) == false)
								{
									errors.Add($"Group {itemInGroup.GroupCode} specified in link for item {itemInGroup.ItemCode} not found in classifier {type.Code}.");
								}

								if (existingItems.TryGetValue(itemInGroup.ItemCode, out var dbItem) == false)
								{
									errors.Add($"Item {itemInGroup.ItemCode} specified in link for group {itemInGroup.GroupCode} not found in classifier {type.Code}.");
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

				// scope.Commit();
			}

			return Guid.Empty;
		}

		private class ClosureTable
		{
			private readonly DbContext _db;

			public ClosureTable(DbContext db)
			{
				_db = db;
			}

			public async Task<long> Insert(Guid itemUid, Guid? parentUid, CancellationToken cancellationToken)
			{
				// insert self closure with level 0
				await _db.GetTable<DbClassifierClosure>()
					.Value(x => x.ParentUid, itemUid)
					.Value(x => x.ChildUid, itemUid)
					.Value(x => x.Level, 0)
					.InsertAsync(cancellationToken);

				if (parentUid.HasValue)
				{
					// insert parent closures with level + 1
					var closures = await _db.GetTable<DbClassifierClosure>()
						.Where(x => x.ChildUid == parentUid.Value)
						.Select(x => new DbClassifierClosure
						{
							ParentUid = x.ParentUid,
							ChildUid = itemUid,
							Level = (short)(x.Level + 1)
						})
						.ToListAsync(cancellationToken);

					return _db.BulkCopy(closures).RowsCopied + 1;
				}

				return 1;
			}
		} 
	}
}
