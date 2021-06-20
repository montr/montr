using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;
using Montr.Kompany.Impl.QueryHandlers;
using Montr.Kompany.Queries;

namespace Montr.Kompany.Tests.QueryHandlers
{
	[TestClass]
	public class GetUserCompanyListHandlerTests
	{
		[TestMethod]
		public async Task GetUserCompanyList_Should_ReturnCompanyList()
		{
			// arrange
			var dbContextFactory = new DefaultDbContextFactory();
			var handler = new GetUserCompanyListHandler(dbContextFactory);

			// act
			var command = new GetUserCompanyList
			{
				UserUid = Guid.NewGuid()
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Count > 0); // todo - fixit - add user with one or more companies
		}
	}
}
