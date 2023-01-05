using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Kompany.Models;
using Montr.MasterData.Tests.Services;

namespace Montr.Kompany.Tests.Services
{
	public class CompanyDbGenerator
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly MasterDataDbGenerator _masterDataDbGenerator;

		public CompanyDbGenerator(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
			_masterDataDbGenerator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
		}

		public async Task<ApiResult> InsertCompany(Company company, CancellationToken cancellationToken)
		{
			await _masterDataDbGenerator.EnsureClassifierTypeRegistered(Company.GetDefaultMetadata(), cancellationToken);

			var companyRepositoryFactory = CompanyMockHelper.CreateClassifierRepositoryFactory(_dbContextFactory);

			var companyRepository = companyRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.Company);

			return await companyRepository.Insert(company, cancellationToken);
		}
	}
}
