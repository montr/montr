using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Services.Implementations;
using Montr.Idx.Models;
using Montr.Kompany.Entities;
using Montr.Kompany.Models;
using Montr.Kompany.Queries;
using Montr.Kompany.Services.QueryHandlers;
using Montr.Kompany.Tests.Services;
using Montr.MasterData.Tests.Services;
using NUnit.Framework;

namespace Montr.Kompany.Tests.QueryHandlers
{
	[TestFixture]
	public class GetUserCompanyListHandlerTests
	{
		[Test]
		public async Task GetUserCompanyList_Should_ReturnCompanyList()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierRepositoryFactory = CompanyMockHelper.CreateClassifierRepositoryFactory(dbContextFactory);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new GetUserCompanyListHandler(dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				await generator.EnsureClassifierTypeRegistered(Company.GetDefaultMetadata(), cancellationToken);
				await generator.EnsureClassifierTypeRegistered(User.GetDefaultMetadata(), cancellationToken);

				var companyRepository = classifierRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.Company);
				var userRepository = classifierRepositoryFactory.GetNamedOrDefaultService(Idx.ClassifierTypeCode.User);

				var company = await companyRepository.Insert(new Company
				{
					Name = "Company 1",
					ConfigCode = CompanyConfigCode.Company
				}, cancellationToken);

				Assert.IsTrue(company.Success);
				Assert.IsNotNull(company.Uid);

				var user = await userRepository.Insert(new User { Name = "User 1" }, cancellationToken);

				Assert.IsTrue(user.Success);
				Assert.IsNotNull(user.Uid);

				using (var db = dbContextFactory.Create())
				{
					await db.GetTable<DbCompanyUser>()
						.Value(x => x.CompanyUid, company.Uid)
						.Value(x => x.UserUid, user.Uid)
						.InsertAsync(cancellationToken);
				}

				// act
				var result = await handler.Handle(new GetUserCompanyList
				{
					UserUid = user.Uid.Value
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.Count);
			}
		}
	}
}
