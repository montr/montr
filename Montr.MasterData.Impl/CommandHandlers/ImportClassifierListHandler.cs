using System;
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

			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, request.TypeCode, cancellationToken);
			
			// using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var data = request.Data;

					var closure = new ClosureTable(db);

					if (data.Items != null)
					{
						var sortedItems = DirectedAcyclicGraphVerifier.TopologicalSort(data.Items,
							node => node.Code, node => node.ParentCode != null ? new [] { node.ParentCode } : null );

						foreach (var item in sortedItems)
						{
							var dbItem = db.GetTable<DbClassifier>()
								.SingleOrDefault(x =>
									x.TypeUid == type.Uid &&
									x.Code == item.Code);

							if (dbItem == null)
							{
								var itemUid = Guid.NewGuid();

								var stmt = db.GetTable<DbClassifier>()
									.Value(x => x.Uid, itemUid)
									.Value(x => x.TypeUid, type.Uid)
									.Value(x => x.StatusCode, item.StatusCode)
									.Value(x => x.Code, item.Code)
									.Value(x => x.Name, item.Name);

								DbClassifier dbParentItem = null;

								if (type.HierarchyType == HierarchyType.Items && item.ParentCode != null)
								{
									dbParentItem = db.GetTable<DbClassifier>()
										.Single(x =>
											x.TypeUid == type.Uid &&
											x.Code == item.ParentCode);

									stmt = stmt.Value(x => x.ParentUid, dbParentItem.Uid);
								}

								await stmt.InsertAsync(cancellationToken);

								await closure.Insert(itemUid, dbParentItem?.Uid, cancellationToken);
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

						if (data.Groups != null)
						{
							var sortedGroups = DirectedAcyclicGraphVerifier.TopologicalSort(data.Groups,
								node => node.Code, node => node.ParentCode != null ? new [] { node.ParentCode } : null );

							foreach (var group in sortedGroups)
							{
								var dbGroup = db
									.GetTable<DbClassifierGroup>()
									.SingleOrDefault(x =>
										x.TreeUid == treeUid &&
										x.Code == group.Code);

								if (dbGroup == null)
								{
									Guid? parentUid = null;

									if (group.ParentCode != null)
									{
										var parentGroup = db
											.GetTable<DbClassifierGroup>()
											.Single(x =>
												x.TreeUid == treeUid &&
												x.Code == group.ParentCode);

										parentUid = parentGroup.Uid;
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

						if (data.Links != null)
						{
							foreach (var itemInGroup in data.Links)
							{
								var dbGroup = db.GetTable<DbClassifierGroup>()
									.Single(x =>
										x.TreeUid == treeUid &&
										x.Code == itemInGroup.GroupCode);

								var dbItem = db.GetTable<DbClassifier>()
									.Single(x =>
										x.TypeUid == type.Uid &&
										x.Code == itemInGroup.ItemCode);

								var dbLink = db.GetTable<DbClassifierLink>()
									.SingleOrDefault(x =>
										x.GroupUid == dbGroup.Uid &&
										x.ItemUid == dbItem.Uid);

								if (dbLink == null)
								{
									await db.GetTable<DbClassifierLink>()
										.Value(x => x.GroupUid, dbGroup.Uid)
										.Value(x => x.ItemUid, dbItem.Uid)
										.InsertAsync(cancellationToken);
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

			public async Task Insert(Guid itemUid, Guid? parentUid, CancellationToken cancellationToken)
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

					/*var copied =*/ _db.BulkCopy(closures);
				}
			}
		} 
	}
}
