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
	public class GetClassifierTreeListHandlerTests
	{
		[TestMethod]
		public async Task GetClassifierTreeList_ReturnList()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTreeRepository = new DbClassifierTreeRepository(dbContextFactory);
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var handler = new GetClassifierTreeListHandler(classifierTreeRepository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// default tree will be insered for HierarchyType.Groups
				await dbHelper.InsertType(HierarchyType.Groups, cancellationToken);

				// act
				var result = await handler.Handle(new GetClassifierTreeList
				{
					CompanyUid = dbHelper.CompanyUid,
					UserUid = dbHelper.UserUid,
					TypeCode = dbHelper.TypeCode
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.TotalCount);
			}
		}
	}
}
