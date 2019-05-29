using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestClass]
	public class GetClassifierListHandlerTests
	{
		[TestMethod]
		public async Task GetClassifierList_Should_ReturnList()
		{
			// todo: convert to classifier repository tests

			// arrange
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			var classifierRepository = new DbClassifierRepository(dbContextFactory, classifierTypeService);

			var handler = new GetClassifierListHandler(classifierRepository);

			// act
			var command = new GetClassifierList
			{
				UserUid = Guid.NewGuid(),
				Request = new ClassifierSearchRequest
				{
					CompanyUid = Constants.OperatorCompanyUid,
					TypeCode = "okei"
				}
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
		}
	}
}
