using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Commands;
using Montr.Core.Impl.CommandHandlers;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Core.Tests.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Tests.CommandHandlers
{
	[TestClass]
	public class DeleteEntityStatusHandlerTests
	{
		[TestMethod]
		public async Task Execute_Should_DeleteEntityStatus()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var generator = new CoreDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new DeleteEntityStatusHandler(unitOfWorkFactory, dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				for (var i = 0; i < 42; i++)
				{
					await generator.InsertEntityStatus(new EntityStatus
					{
						Code = "status_code_" + i,
						Name = "Status Name " + i
					}, cancellationToken);
				}

				// act
				var request = new DeleteEntityStatus
				{
					EntityTypeCode = generator.EntityTypeCode,
					EntityUid = generator.EntityUid,
					Codes = new[] { "status_code_1", "status_code_2", "status_code_12" }
				};

				var result = await handler.Handle(request, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(3, result.AffectedRows);

				var statuses = await generator.GetEntityStatuses(cancellationToken);
				
				Assert.AreEqual(39, statuses.Rows.Count);

				foreach (var code in request.Codes)
				{
					Assert.IsNull(statuses.Rows.FirstOrDefault(x => x.Code == code));
				}
			}
		}
	}
}
