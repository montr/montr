using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Kompany.Impl.Entities;
using Montr.Kompany.Models;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Services;

namespace Montr.Kompany.Impl.Services
{
	public class DbCompanyRepository : DbClassifierRepository<Company>
	{
		public DbCompanyRepository(IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService, IClassifierTreeService classifierTreeService,
			IClassifierTypeMetadataService metadataService, IFieldDataRepository fieldDataRepository, INumberGenerator numberGenerator)
			: base(dbContextFactory, classifierTypeService, classifierTreeService, metadataService, fieldDataRepository, numberGenerator)
		{
		}

		protected override async Task<SearchResult<Classifier>> SearchInternal(DbContext db,
			ClassifierType type, ClassifierSearchRequest request, CancellationToken cancellationToken)
		{
			var dbClassifiers = BuildQuery(db, type, request);

			var dbCompanies = db.GetTable<DbCompany>().AsQueryable();

			if (request is CompanySearchRequest companyRequest)
			{
				if (companyRequest.ConfigCode != null)
				{
					dbCompanies = dbCompanies.Where(x => x.ConfigCode == companyRequest.ConfigCode);
				}
			}

			var joined = from classifier in dbClassifiers
				join company in dbCompanies on classifier.Uid equals company.Uid
				select new DbItem { Classifier = classifier, Company = company };

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

		private Company Materialize(ClassifierType type, DbItem dbItem)
		{
			var item = base.Materialize(type, dbItem.Classifier);

			var dbCompany = dbItem.Company;

			item.ConfigCode = dbCompany.ConfigCode;

			return item;
		}

		protected override async Task<ApiResult> InsertInternal(DbContext db,
			ClassifierType type, Classifier item, CancellationToken cancellationToken = default)
		{
			var result = await base.InsertInternal(db, type, item, cancellationToken);

			if (result.Success)
			{
				var company = (Company)item;

				await db.GetTable<DbCompany>()
					.Value(x => x.Uid, item.Uid)
					.Value(x => x.ConfigCode, company.ConfigCode)
					.InsertAsync(cancellationToken);
			}

			return result;
		}

		protected override async Task<ApiResult> UpdateInternal(DbContext db,
			ClassifierType type, ClassifierTree tree, Classifier item, CancellationToken cancellationToken = default)
		{
			var result = await base.UpdateInternal(db, type, tree, item, cancellationToken);

			if (result.Success)
			{
				var company = (Company)item;

				await db.GetTable<DbCompany>()
					.Where(x => x.Uid == item.Uid)
					.Set(x => x.ConfigCode, company.ConfigCode)
					.UpdateAsync(cancellationToken);
			}

			return result;
		}

		protected override async Task<ApiResult> DeleteInternal(DbContext db,
			ClassifierType type, DeleteClassifier request, CancellationToken cancellationToken = default)
		{
			await db.GetTable<DbCompany>()
				.Where(x => request.Uids.Contains(x.Uid))
				.DeleteAsync(cancellationToken);

			return await base.DeleteInternal(db, type, request, cancellationToken);
		}

		private class DbItem
		{
			public DbClassifier Classifier { get; init; }

			public DbCompany Company { get; init; }
		}
	}
}
