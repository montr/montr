using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestClass]
	public class GetClassifierGroupListHandlerTests
	{
		[TestMethod]
		public async Task GetClassifierGroupList_Should_ReturnList()
		{
			// arrange
			var dbContextFactory = new DefaultDbContextFactory();

			var handler = new GetClassifierGroupListHandler(dbContextFactory);

			// act
			var command = new GetClassifierGroupList
			{
				Request = new ClassifierGroupSearchRequest
				{
					CompanyUid = Guid.Parse("6465dd4c-8664-4433-ba6a-14effd40ebed"),
					TypeCode = "okei",
					TreeCode = "default"
				}
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(4, result.Count);
			foreach (var group in result)
			{
				Assert.IsNotNull(group.Children);
				Assert.IsTrue(group.Children.Count >= 3);
			}
		}
	}
}
