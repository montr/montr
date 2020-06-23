using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Automate.Commands;
using Montr.Automate.Impl.CommandHandlers;
using Montr.Automate.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Automate.Tests.CommandHandlers
{
	[TestClass]
	public class UpdateAutomationHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_UpdateAutomation()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var jsonSerializer = new NewtonsoftJsonSerializer();
			var handler = new UpdateAutomationHandler(unitOfWorkFactory, dbContextFactory, jsonSerializer);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange

				// act
				var command = new UpdateAutomation
				{
					Item = new Automation
					{
					}
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.AffectedRows);
			}
		}
	}
}
