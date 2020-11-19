using System;
using System.Collections.Generic;
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
	public class ClassifierTypeProvider : IClassifierTypeProvider
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

		public virtual Type ClassifierType => throw new NotImplementedException();

		public virtual async Task<SearchResult<Classifier>> Search(ClassifierType type,
			ClassifierSearchRequest request, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				var query = Search(db, type, request);

				List<Classifier> data;

				if (type.Code == NumeratorTypeProvider.TypeCode)
				{
					data = new List<Classifier>();

					var joined = from c in query
						join t in db.GetTable<DbNumerator>() on c.Uid equals t.Uid
						select new { Classifier = c, Numerator = t };

					// todo: fix paging
					if (request.SortColumn == null) request.SortColumn = "Code";
					request.SortColumn = nameof(Classifier) + "." + request.SortColumn;

					foreach (var row in joined.Apply(request, x => x.Classifier.Code))
					{
						var item = new Numerator();

						Materialize(type, item, row.Classifier);

						Materialize(item, row.Numerator);

						data.Add(item);
					}
				}
				else
				{
					data = await Materialize(
						query.Apply(request, x => x.Code), type, cancellationToken);
				}

				// todo: add test
				if (request.FocusUid.HasValue && data.Any(x => x.Uid == request.FocusUid) == false)
				{
					// todo:
					if (type.Code == NumeratorTypeProvider.TypeCode)
					{
					}
					else
					{
						var focused = await Materialize(
							query.Where(x => x.Uid == request.FocusUid),
							type, cancellationToken);

						data.InsertRange(0, focused);
					}
				}

				// todo: preload fields for multiple items
				if (request.IncludeFields)
				{
					var metadata = await _metadataService.GetMetadata(type, cancellationToken);

					foreach (var item in data)
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

				return new SearchResult<Classifier>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}

		private static IQueryable<DbClassifier> Search(DbContext db, ClassifierType type, ClassifierSearchRequest request)
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

		private static async Task<List<Classifier>> Materialize(IQueryable<DbClassifier> query, ClassifierType type, CancellationToken cancellationToken)
		{
			return await query
				.Select(x => Materialize(type, new Classifier(), x))
				.ToListAsync(cancellationToken);
		}

		private static Classifier Materialize(ClassifierType type, Classifier item, DbClassifier dbItem)
		{
			item.Type = type.Code;
			item.Uid = dbItem.Uid;
			item.StatusCode = dbItem.StatusCode;
			item.Code = dbItem.Code;
			item.Name = dbItem.Name;
			item.ParentUid = dbItem.ParentUid;
			item.Url = $"/classifiers/{type.Code}/edit/{dbItem.Uid}";

			return item;
		}

		private static Numerator Materialize(Numerator item, DbNumerator dbItem)
		{
			item.EntityTypeCode = dbItem.EntityTypeCode;
			item.Periodicity = Enum.Parse<NumeratorPeriodicity>(dbItem.Periodicity);
			item.Pattern = dbItem.Pattern;
			item.KeyTags = dbItem.KeyTags?.Split(DbNumerator.KeyTagsSeparator, StringSplitOptions.RemoveEmptyEntries);
			item.IsActive = dbItem.IsActive;
			item.IsSystem = dbItem.IsSystem;

			return item;
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

	public class NumeratorTypeProvider : ClassifierTypeProvider
	{
		public static readonly string TypeCode = nameof(Numerator).ToLower();

		private readonly IDbContextFactory _dbContextFactory;

		public NumeratorTypeProvider(IDbContextFactory dbContextFactory,
			IClassifierTypeMetadataService metadataService, IFieldDataRepository fieldDataRepository)
			: base(dbContextFactory, metadataService, fieldDataRepository)
		{
			_dbContextFactory = dbContextFactory;
		}

		public override Type ClassifierType { get; } = typeof(Numerator);

		public override Task<Classifier> Create(ClassifierType type, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public override async Task Insert(ClassifierType type, Classifier item, CancellationToken cancellationToken)
		{
			var numerator = (Numerator)item;

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbNumerator>()
					.Value(x => x.Uid, item.Uid)
					.Value(x => x.EntityTypeCode, numerator.EntityTypeCode)
					// .Value(x => x.Name, numerator.Name)
					.Value(x => x.Pattern, numerator.Pattern)
					.Value(x => x.Periodicity, numerator.Periodicity.ToString())
					.Value(x => x.IsActive, numerator.IsActive)
					.Value(x => x.IsSystem, numerator.IsSystem)
					.InsertAsync(cancellationToken);
			}
		}

		public override async Task Update(ClassifierType type, Classifier item, CancellationToken cancellationToken)
		{
			var numerator = (Numerator)item;

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbNumerator>()
					.Where(x => x.Uid == item.Uid)
					// .Set(x => x.Name, item.Name)
					.Set(x => x.Pattern, numerator.Pattern)
					.Set(x => x.Periodicity, numerator.Periodicity.ToString())
					.Set(x => x.IsActive, numerator.IsActive)
					// .Set(x => x.IsSystem, item.IsSystem)
					.UpdateAsync(cancellationToken);
			}
		}
	}
}
