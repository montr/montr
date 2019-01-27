using System;
using System.Threading;
using System.Threading.Tasks;
using Kompany.Implementation.QueryHandlers;
using Kompany.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;

namespace Kompany.Tests.QueryHandlers
{
	[TestClass]
	public class GetCompanyListHandlerTests
	{
		[TestMethod]
		public async Task GetCompanyList_Should_ReturnCompanyList()
		{
			// arrange
			TestHelper.InitDb();

			var dbContextFactory = new DefaultDbContextFactory();

			var handler = new GetCompanyListHandler(dbContextFactory);

			// act
			var command = new GetCompanyList
			{
				UserUid = Guid.NewGuid()
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
		}
	}
}
