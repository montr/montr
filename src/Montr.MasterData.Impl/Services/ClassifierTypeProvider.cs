using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.MasterData.Impl.Services
{
	public class ClassifierTypeProvider<T> : IClassifierTypeProvider where T : Classifier, new()
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeMetadataService _metadataService;
		private readonly IFieldDataRepository _fieldDataRepository;

		public ClassifierTypeProvider(IDbContextFactory dbContextFactory,
			IClassifierTypeMetadataService metadataService, IFieldDataRepository fieldDataRepository)
		{
			_dbContextFactory = dbContextFactory;
			_metadataService = metadataService;
			_fieldDataRepository = fieldDataRepository;
		}

		public Type ClassifierType => typeof(T);

		public async Task<SearchResult<Classifier>> Search(ClassifierType type,
			ClassifierSearchRequest request, CancellationToken cancellationToken)
		{
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
			return new T
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

		public virtual Task<Classifier> Create(ClassifierType type, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public virtual Task Insert(ClassifierType type, Classifier item, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public virtual Task Update(ClassifierType type, Classifier item, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
