using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;
using Montr.Tendr.Impl.QueryHandlers;
using Montr.Tendr.Queries;

namespace Montr.Tendr.Tests.QueryHandlers
{
	[TestClass]
	public class GetEventListHandlerTests
	{
		[TestMethod]
		public async Task GetEventList_ForNormalRequest_ReturnItems()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();

			var handler = new GetEventListHandler(dbContextFactory);

			// act
			var command = new GetEventList
			{
				UserUid = Guid.NewGuid(),
			};

			var result = await handler.Handle(command, cancellationToken);

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Rows);
		}
	}
}
