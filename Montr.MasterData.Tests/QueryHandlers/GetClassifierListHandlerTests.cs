using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestClass]
	public class GetClassifierListHandlerTests
	{
		[TestMethod]
		public async Task GetClassifierList_Should_ReturnList()
		{
			// todo: split test

			// arrange
			var dbContextFactory = new DefaultDbContextFactory();
			var repository = new ClassifierRepository(dbContextFactory);

			var handler = new GetClassifierListHandler(repository);

			// act
			var command = new GetClassifierList
			{
				UserUid = Guid.NewGuid()
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
		}
	}
}
