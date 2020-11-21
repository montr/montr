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
using Montr.Metadata.Services;

namespace Montr.MasterData.Impl.Services
{
	public class NumeratorTypeProvider : ClassifierTypeProvider<Numerator>
	{
		public static readonly string TypeCode = nameof(Numerator).ToLower();

		private readonly IDbContextFactory _dbContextFactory;

		public NumeratorTypeProvider(IDbContextFactory dbContextFactory,
			IClassifierTypeMetadataService metadataService, IFieldDataRepository fieldDataRepository)
			: base(dbContextFactory, metadataService, fieldDataRepository)
		{
			_dbContextFactory = dbContextFactory;
		}

		protected override async Task<SearchResult<Classifier>> SearchInternal(DbContext db,
			ClassifierType type, ClassifierSearchRequest request, CancellationToken cancellationToken)
		{
			var query = BuildQuery(db, type, request);

			var joined = from c in query
				join t in db.GetTable<DbNumerator>() on c.Uid equals t.Uid
				select new DbItem { Classifier = c, Numerator = t };

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
				TotalCount = query.GetTotalCount(request),
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
			item.IsActive = dbNumerator.IsActive;
			item.IsSystem = dbNumerator.IsSystem;

			return item;
		}

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

		private class DbItem
		{
			public DbClassifier Classifier { get; set; }

			public DbNumerator Numerator { get; set; }
		}
	}
}
