using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;

namespace Montr.MasterData.Impl.CommandHandlers
{
	// todo: move to service? should recieve stream?
	public class ImportClassifierListHandler : IRequestHandler<ImportClassifierList, Guid>
	{
		private const string DefaultTreeCode = "default";

		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<ClassifierType> _classifierTypeRepository;

		public ImportClassifierListHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IRepository<ClassifierType> classifierTypeRepository)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_classifierTypeRepository = classifierTypeRepository;
		}

		public async Task<Guid> Handle(ImportClassifierList request, CancellationToken cancellationToken)
		{
			var companyUid = request.CompanyUid;
			var userUid = request.UserUid;
			var data = request.Data;

			// todo: start with unit test for each todo
			// todo: build DAG for groups and items
			// todo: build dictionaries
			// todo: generate codes for new entities
			// todo: add new items to default tree
			// todo: build tree of items
			// todo: build closure table for groups or item

			var types = await _classifierTypeRepository.Search(
				new ClassifierTypeSearchRequest
				{
					CompanyUid = companyUid,
					UserUid = userUid,
					Code = request.TypeCode
				}, cancellationToken);

			var type = types.Rows.Single();
			
			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					if (data.Items != null)
					{
						foreach (var item in data.Items)
						{
							var dbItem = db.GetTable<DbClassifier>()
								.SingleOrDefault(x =>
									x.TypeUid == type.Uid &&
									x.Code == item.Code);

							if (dbItem == null)
							{
								var stmt = db.GetTable<DbClassifier>()
									.Value(x => x.Uid, Guid.NewGuid())
									.Value(x => x.TypeUid, type.Uid)
									.Value(x => x.StatusCode, ClassifierStatusCode.Draft)
									.Value(x => x.Code, item.Code)
									.Value(x => x.Name, item.Name);

								if (type.HierarchyType == HierarchyType.Items && item.ParentCode != null)
								{
									var dbParentItem = db.GetTable<DbClassifier>()
										.Single(x =>
											x.TypeUid == type.Uid &&
											x.Code == item.ParentCode);

									stmt = stmt.Value(x => x.ParentUid, dbParentItem.Uid);
								}

								await stmt.InsertAsync(cancellationToken);
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
							foreach (var group in data.Groups)
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

									await db.GetTable<DbClassifierGroup>()
										.Value(x => x.Uid, Guid.NewGuid())
										.Value(x => x.TreeUid, treeUid)
										.Value(x => x.Code, group.Code)
										.Value(x => x.Name, group.Name)
										.Value(x => x.ParentUid, parentUid)
										.InsertAsync(cancellationToken);
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

				scope.Commit();
			}

			return Guid.Empty;
		}
	}
}
