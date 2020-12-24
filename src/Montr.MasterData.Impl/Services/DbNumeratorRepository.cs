using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Services;

namespace Montr.MasterData.Impl.Services
{
	public class DbNumeratorRepository : DbClassifierRepository<Numerator>
	{
		public static readonly string TypeCode = nameof(Numerator).ToLower();

		public DbNumeratorRepository(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IDateTimeProvider dateTimeProvider, IClassifierTypeService classifierTypeService,
			IClassifierTreeService classifierTreeService, IClassifierTypeMetadataService metadataService,
			IFieldDataRepository fieldDataRepository, INumberGenerator numberGenerator)
			: base(unitOfWorkFactory, dbContextFactory, dateTimeProvider, classifierTypeService, classifierTreeService,
				metadataService, fieldDataRepository, numberGenerator)
		{
		}

		protected override async Task<SearchResult<Classifier>> SearchInternal(DbContext db,
			ClassifierType type, ClassifierSearchRequest request, CancellationToken cancellationToken)
		{
			var classifiers = BuildQuery(db, type, request);

			var numerators = db.GetTable<DbNumerator>().AsQueryable();

			var numeratorRequest = request as NumeratorSearchRequest;

			if (numeratorRequest?.EntityTypeCode != null && numeratorRequest.EntityTypeUid != null)
			{
				numerators = from n in numerators.Where(x => x.EntityTypeCode == numeratorRequest.EntityTypeCode)
					join ne in db.GetTable<DbNumeratorEntity>().Where(x => x.EntityUid == numeratorRequest.EntityTypeUid)
						on n.Uid equals ne.NumeratorUid
					select n;
			}

			var joined = from classifier in classifiers
				join numerator in numerators on classifier.Uid equals numerator.Uid
				select new DbItem { Classifier = classifier, Numerator = numerator };

			// todo: fix paging - map column to expression
			request.SortColumn ??= nameof(Classifier.Code);
			request.SortColumn = nameof(DbItem.Classifier) + "." + request.SortColumn;

			var data = await joined
				.Apply(request, x => x.Classifier.Code)
				.Select(x => Materialize(type, x))
				.Cast<Classifier>()
				.ToListAsync(cancellationToken);

			return new SearchResult<Classifier>
			{
				TotalCount = joined.GetTotalCount(request),
				Rows = data
			};
		}

		private Numerator Materialize(ClassifierType type, DbItem dbItem)
		{
			var item = base.Materialize(type, dbItem.Classifier);

			var dbNumerator = dbItem.Numerator;

			item.EntityTypeCode = dbNumerator.EntityTypeCode;
			item.Periodicity = Enum.Parse<NumeratorPeriodicity>(dbNumerator.Periodicity);
			item.Pattern = dbNumerator.Pattern;
			item.KeyTags = dbNumerator.KeyTags?.Split(DbNumerator.KeyTagsSeparator, StringSplitOptions.RemoveEmptyEntries);

			return item;
		}

		protected override async Task<Numerator> CreateInternal(ClassifierCreateRequest request, CancellationToken cancellationToken)
		{
			var result = await base.CreateInternal(request, cancellationToken);

			result.EntityTypeCode = Models.ClassifierType.TypeCode;
			result.Periodicity = NumeratorPeriodicity.Year;
			result.Pattern = "{Number}";

			return result;
		}

		protected override async Task<ApiResult> InsertInternal(
			DbContext db, ClassifierType type, Classifier item, CancellationToken cancellationToken)
		{
			var result = await base.InsertInternal(db, type, item, cancellationToken);

			if (result.Success)
			{
				var numerator = (Numerator) item;

				await db.GetTable<DbNumerator>()
					.Value(x => x.Uid, item.Uid)
					.Value(x => x.EntityTypeCode, numerator.EntityTypeCode)
					.Value(x => x.Pattern, numerator.Pattern)
					.Value(x => x.Periodicity, numerator.Periodicity.ToString())
					.InsertAsync(cancellationToken);
			}

			return result;
		}

		protected override async Task<ApiResult> UpdateInternal(
			DbContext db, ClassifierType type, ClassifierTree tree, Classifier item, CancellationToken cancellationToken)
		{
			var result = await base.UpdateInternal(db, type, tree, item, cancellationToken);

			if (result.Success)
			{
				var numerator = (Numerator)item;

				await db.GetTable<DbNumerator>()
					.Where(x => x.Uid == item.Uid)
					.Set(x => x.Pattern, numerator.Pattern)
					.Set(x => x.Periodicity, numerator.Periodicity.ToString())
					.UpdateAsync(cancellationToken);
			}

			return result;
		}

		protected override async Task<ApiResult> DeleteInternal(DbContext db, ClassifierType type,
			DeleteClassifier request, CancellationToken cancellationToken)
		{
			await db.GetTable<DbNumeratorCounter>()
				.Where(x => request.Uids.Contains(x.NumeratorUid))
				.DeleteAsync(cancellationToken);

			// todo: validate numerator is not used in entities?
			await db.GetTable<DbNumeratorEntity>()
				.Where(x => x.NumeratorUid != null && request.Uids.Contains(x.NumeratorUid.Value))
				.DeleteAsync(cancellationToken);

			await db.GetTable<DbNumerator>()
				.Where(x => request.Uids.Contains(x.Uid))
				.DeleteAsync(cancellationToken);

			return await base.DeleteInternal(db, type, request, cancellationToken);
		}

		private class DbItem
		{
			public DbClassifier Classifier { get; init; }

			public DbNumerator Numerator { get; init; }
		}
	}
}
