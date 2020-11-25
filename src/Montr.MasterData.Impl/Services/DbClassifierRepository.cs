using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.MasterData.Impl.Services
{
	public class DbClassifierRepository<T> : IClassifierRepository where T : Classifier, new()
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IClassifierTreeService _classifierTreeService;
		private readonly IClassifierTypeMetadataService _metadataService;
		private readonly IFieldDataRepository _fieldDataRepository;
		private readonly INumberGenerator _numberGenerator;

		public DbClassifierRepository(
			IUnitOfWorkFactory unitOfWorkFactory,
			IDbContextFactory dbContextFactory,
			IDateTimeProvider dateTimeProvider,
			IClassifierTypeService classifierTypeService,
			IClassifierTreeService classifierTreeService,
			IClassifierTypeMetadataService metadataService,
			IFieldDataRepository fieldDataRepository,
			INumberGenerator numberGenerator)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
			_classifierTypeService = classifierTypeService;
			_classifierTreeService = classifierTreeService;
			_metadataService = metadataService;
			_fieldDataRepository = fieldDataRepository;
			_numberGenerator = numberGenerator;
		}

		public Type ClassifierType => typeof(T);

		public async Task<SearchResult<Classifier>> Search(ClassifierSearchRequest request, CancellationToken cancellationToken)
		{
			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			using (var db = _dbContextFactory.Create())
			{
				var result = await SearchInternal(db, type, request, cancellationToken);

				// search in data by Uid is not effective, but ok for small collections
				if (request.FocusUid.HasValue && result.Rows.Any(x => x.Uid == request.FocusUid) == false)
				{
					// todo: add test
					var focus = await SearchInternal(db, type, new ClassifierSearchRequest
					{
						Uid = request.FocusUid,
						SkipPaging = true
					}, cancellationToken);

					for (var i = focus.Rows.Count - 1; i >= 0; i--)
					{
						result.Rows.Insert(0, focus.Rows[i]);
					}
				}

				// todo: preload fields for multiple items or (?) store fields in the same table?
				if (request.IncludeFields)
				{
					var metadata = await _metadataService.GetMetadata(type, cancellationToken);

					foreach (var item in result.Rows)
					{
						var fields = await _fieldDataRepository.Search(new FieldDataSearchRequest
						{
							Metadata = metadata,
							EntityTypeCode = Classifier.TypeCode,
							// ReSharper disable once PossibleInvalidOperationException
							EntityUids = new[] { item.Uid.Value }
						}, cancellationToken);

						item.Fields = fields.Rows.SingleOrDefault();
					}
				}

				return result;
			}
		}

		/// <summary>
		/// Filter by all parameters except FocusUid
		/// </summary>
		protected virtual async Task<SearchResult<Classifier>> SearchInternal(DbContext db,
			ClassifierType type, ClassifierSearchRequest request, CancellationToken cancellationToken)
		{
			var query = BuildQuery(db, type, request);

			var data = await query
				.Apply(request, x => x.Code)
				.Select(x => Materialize(type, x))
				.Cast<Classifier>()
				.ToListAsync(cancellationToken);

			return new SearchResult<Classifier>
			{
				TotalCount = query.GetTotalCount(request),
				Rows = data
			};
		}

		protected IQueryable<DbClassifier> BuildQuery(DbContext db, ClassifierType type, ClassifierSearchRequest request)
		{
			IQueryable<DbClassifier> query = null;

			if (type.HierarchyType == HierarchyType.Groups)
			{
				if (request.GroupUid != null)
				{
					if (request.Depth == null || request.Depth == "0") // todo: use constant
					{
						query = from trees in db.GetTable<DbClassifierTree>()
							join childrenGroups in db.GetTable<DbClassifierGroup>() on trees.Uid equals childrenGroups.TreeUid
							join links in db.GetTable<DbClassifierLink>() on childrenGroups.Uid equals links.GroupUid
							join c in db.GetTable<DbClassifier>() on links.ItemUid equals c.Uid
							where trees.TypeUid == type.Uid &&
							      trees.Uid == request.TreeUid &&
							      childrenGroups.Uid == request.GroupUid
							select c;
					}
					else
					{
						query = from trees in db.GetTable<DbClassifierTree>()
							join parentGroups in db.GetTable<DbClassifierGroup>() on trees.Uid equals parentGroups.TreeUid
							join closures in db.GetTable<DbClassifierClosure>() on parentGroups.Uid equals closures.ParentUid
							join childrenGroups in db.GetTable<DbClassifierGroup>() on closures.ChildUid equals childrenGroups
								.Uid
							join links in db.GetTable<DbClassifierLink>() on childrenGroups.Uid equals links.GroupUid
							join c in db.GetTable<DbClassifier>() on links.ItemUid equals c.Uid
							where trees.TypeUid == type.Uid &&
							      trees.Uid == request.TreeUid &&
							      parentGroups.Uid == request.GroupUid
							select c;
					}
				}
			}
			else if (type.HierarchyType == HierarchyType.Items)
			{
				if (request.GroupUid != null)
				{
					if (request.Depth == null || request.Depth == "0") // todo: use enum or constant
					{
						query = from parent in db.GetTable<DbClassifier>()
							join @class in db.GetTable<DbClassifier>() on parent.Uid equals @class.ParentUid
							where parent.TypeUid == type.Uid && parent.Uid == request.GroupUid
							select @class;
					}
					else
					{
						query = from parent in db.GetTable<DbClassifier>()
							join closures in db.GetTable<DbClassifierClosure>() on parent.Uid equals closures.ParentUid
							join @class in db.GetTable<DbClassifier>() on closures.ChildUid equals @class.Uid
							where parent.TypeUid == type.Uid && parent.Uid == request.GroupUid && closures.Level > 0
							select @class;
					}
				}
			}

			if (query == null)
			{
				query = from c in db.GetTable<DbClassifier>()
					where c.TypeUid == type.Uid
					select c;
			}

			if (request.Uid != null)
			{
				query = query.Where(x => x.Uid == request.Uid);
			}

			if (request.Uids != null)
			{
				query = query.Where(x => request.Uids.Contains(x.Uid));
			}

			if (request.SearchTerm != null)
			{
				query = query.Where(x => SqlExpr.ILike(x.Name, "%" + request.SearchTerm + "%"));

				// query = query.Where(x => Sql.Like(x.Name, "%" + request.SearchTerm + "%"));
				// query = query.Where(x => x.Name.Contains(request.SearchTerm));
			}

			return query;
		}

		protected T Materialize(ClassifierType type, DbClassifier dbItem)
		{
			return new()
			{
				Type = type.Code,
				Uid = dbItem.Uid,
				StatusCode = dbItem.StatusCode,
				Code = dbItem.Code,
				Name = dbItem.Name,
				ParentUid = dbItem.ParentUid,
				Url = $"/classifiers/{type.Code}/edit/{dbItem.Uid}"
			};
		}

		public async Task<Classifier> Create(ClassifierCreateRequest request, CancellationToken cancellationToken)
		{
			return await CreateInternal(request, cancellationToken);
		}

		protected virtual async Task<T> CreateInternal(ClassifierCreateRequest request, CancellationToken cancellationToken)
		{
			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			var number = await _numberGenerator.GenerateNumber(new GenerateNumberRequest
			{
				EntityTypeCode = Classifier.TypeCode,
				EntityTypeUid = type.Uid
			}, cancellationToken);

			return new()
			{
				Type = type.Code,
				ParentUid = request.ParentUid,
				Code = number
			};
		}

		public virtual async Task<ApiResult> Insert(Classifier item, CancellationToken cancellationToken)
		{
			var now = _dateTimeProvider.GetUtcNow();

			var type = await _classifierTypeService.Get(item.Type, cancellationToken);

			var itemUid = Guid.NewGuid();

			item.Uid = itemUid;

			// todo: validate fields
			var metadata = await _metadataService.GetMetadata(type, cancellationToken);

			var manageFieldDataRequest = new ManageFieldDataRequest
			{
				EntityTypeCode = Classifier.TypeCode,
				EntityUid = itemUid,
				Metadata = metadata,
				Item = item
			};

			// todo: move to ClassifierValidator (?)
			var result = await _fieldDataRepository.Validate(manageFieldDataRequest, cancellationToken);

			if (result.Success == false) return result;

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var validator = new ClassifierValidator(db, type);

					if (await validator.ValidateInsert(item, cancellationToken) == false)
					{
						return new ApiResult { Success = false, Errors = validator.Errors };
					}

					// todo: company + modification data

					// insert classifier
					await db.GetTable<DbClassifier>()
						.Value(x => x.Uid, itemUid)
						.Value(x => x.TypeUid, type.Uid)
						.Value(x => x.StatusCode, ClassifierStatusCode.Active)
						.Value(x => x.Code, item.Code)
						.Value(x => x.Name, item.Name)
						// todo: validate parent belongs to the same classifier
						.Value(x => x.ParentUid, type.HierarchyType == HierarchyType.Items ? item.ParentUid : null)
						.InsertAsync(cancellationToken);

					if (type.HierarchyType == HierarchyType.Groups)
					{
						// todo: validate group belongs to the same classifier
						// todo: validate selected group belong to default tree

						// what if insert classifier and no groups inserted before?
						/*if (request.TreeUid == null || request.ParentUid == null)
						{
							throw new InvalidOperationException("Classifier should belong to one of the default hierarchy group.");
						}*/

						// todo: should be linked to at least one main group?
						if (/*request.TreeUid != null &&*/ item.ParentUid != null)
						{
							// link to selected group
							await db.GetTable<DbClassifierLink>()
								.Value(x => x.GroupUid, item.ParentUid)
								.Value(x => x.ItemUid, itemUid)
								.InsertAsync(cancellationToken);
						}

						// if group is not of default hierarchy, link to default hierarchy root
						/*var root = await GetRoot(db, request.ParentUid.Value, cancellationToken);

						if (root.Code != ClassifierTree.DefaultCode)
						{
							await LinkToDefaultRoot(db, type, itemUid, cancellationToken);
						}*/
					}
					else if (type.HierarchyType == HierarchyType.Items)
					{
						var closureTable = new ClosureTableHandler(db, type);

						if (await closureTable.Insert(itemUid, item.ParentUid, cancellationToken) == false)
						{
							return new ApiResult { Success = false, Errors = closureTable.Errors };
						}
					}
				}

				// insert fields
				// todo: exclude db fields and sections
				await _fieldDataRepository.Insert(manageFieldDataRequest, cancellationToken);

				// insert to specific classifier table
				// if (classifierTypeProvider != null) await classifierTypeProvider.Insert(type, item, cancellationToken);

				await InsertInternal(type, item, cancellationToken);

				// todo: events

				scope.Commit();

				return new ApiResult { Uid = itemUid };
			}
		}

		protected virtual Task InsertInternal(ClassifierType type, Classifier item, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public virtual async Task<ApiResult> Update(Classifier item, CancellationToken cancellationToken)
		{
			var type = await _classifierTypeService.Get(item.Type, cancellationToken);

			var tree = type.HierarchyType == HierarchyType.Groups
				? await _classifierTreeService.GetClassifierTree(/*request.CompanyUid,*/ type.Code, ClassifierTree.DefaultCode, cancellationToken)
				: null;

			// todo: validate fields
			var metadata = await _metadataService.GetMetadata(type, cancellationToken);

			var manageFieldDataRequest = new ManageFieldDataRequest
			{
				EntityTypeCode = Classifier.TypeCode,
				// ReSharper disable once PossibleInvalidOperationException
				EntityUid = item.Uid.Value,
				Metadata = metadata,
				Item = item
			};

			// todo: move to ClassifierValidator (?)
			var result = await _fieldDataRepository.Validate(manageFieldDataRequest, cancellationToken);

			if (result.Success == false)
			{
				return result;
			}

			// var classifierTypeProvider = _classifierTypeProviderFactory?.GetService(type.Code);

			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					var validator = new ClassifierValidator(db, type);

					if (await validator.ValidateUpdate(item, cancellationToken) == false)
					{
						return new ApiResult { Success = false, Errors = validator.Errors };
					}

					affected = await db.GetTable<DbClassifier>()
						.Where(x => x.Uid == item.Uid)
						.Set(x => x.Code, item.Code)
						.Set(x => x.Name, item.Name)
						.Set(x => x.ParentUid, type.HierarchyType == HierarchyType.Items ? item.ParentUid : null)
						.UpdateAsync(cancellationToken);

					if (type.HierarchyType == HierarchyType.Groups)
					{
						// todo: combine with InsertClassifierLinkHandler in one service

						// delete other links in same tree
						var deleted = await (
							from link in db.GetTable<DbClassifierLink>().Where(x => x.ItemUid == item.Uid)
							join groups in db.GetTable<DbClassifierGroup>() on link.GroupUid equals groups.Uid
							where groups.TreeUid == tree.Uid
							select link
						).DeleteAsync(cancellationToken);

						// todo: check parent belongs to default tree
						if (item.ParentUid != null)
						{
							var inserted = await db.GetTable<DbClassifierLink>()
								.Value(x => x.GroupUid, item.ParentUid)
								.Value(x => x.ItemUid, item.Uid)
								.InsertAsync(cancellationToken);
						}
					}
					else if (type.HierarchyType == HierarchyType.Items)
					{
						var closureTable = new ClosureTableHandler(db, type);

						// ReSharper disable once PossibleInvalidOperationException
						if (await closureTable.Update(item.Uid.Value, item.ParentUid, cancellationToken) == false)
						{
							return new ApiResult { Success = false, Errors = closureTable.Errors };
						}
					}
				}

				// update fields
				await _fieldDataRepository.Update(manageFieldDataRequest, cancellationToken);

				// update specific classifier table
				// if (classifierTypeProvider != null) await classifierTypeProvider.Update(type, item, cancellationToken);

				await UpdateInternal(type, item, cancellationToken);

				// todo: events

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}

		protected virtual Task UpdateInternal(ClassifierType type, Classifier item, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
