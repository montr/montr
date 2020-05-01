using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestClass]
	public class GetNumeratorListHandlerTests
	{
		[TestMethod]
		public async Task GetNumeratorList_Should_ReturnList()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbNumeratorRepository = new DbNumeratorRepository(dbContextFactory);
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var handler = new GetNumeratorListHandler(dbNumeratorRepository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				var numerator = await dbHelper.InsertNumerator(
					new Numerator(),
					new GenerateNumberRequest
					{
						EntityTypeCode = "NumeratorTypeCode",
						EntityTypeUid = Guid.NewGuid()
					}, cancellationToken);

				// act
				var command = new GetNumeratorList();
				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Rows.Count > 0);
				Assert.IsTrue(result.Rows.Count(x => x.EntityTypeCode == "NumeratorTypeCode") == 1);
			}
		}
	}
}
