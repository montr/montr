using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Services;

namespace Montr.MasterData.Tests.Services
{
	[TestClass]
	public class DbNumberingServiceTests
	{
		[TestMethod]
		public async Task GenerateNumber()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var service = new DbNumberingService(dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act
				var number = await service.GenerateNumber(Guid.NewGuid(), string.Empty, Guid.NewGuid(), cancellationToken);

				// assert
				Assert.IsNotNull(number);
			}
		}
	}
}
