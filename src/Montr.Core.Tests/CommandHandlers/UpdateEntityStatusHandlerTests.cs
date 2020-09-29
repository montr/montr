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
	public class UpdateEntityStatusHandlerTests
	{
		[TestMethod]
		public async Task Execute_Should_UpdateEntityStatus()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var generator = new CoreDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new UpdateEntityStatusHandler(unitOfWorkFactory, dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertEntityStatus(new EntityStatus { DisplayOrder = 1, Code = "draft", Name = "Draft" }, cancellationToken);
				await generator.InsertEntityStatus(new EntityStatus { DisplayOrder = 2, Code = "published", Name = "Published" }, cancellationToken);

				// act
				var request = new UpdateEntityStatus
				{
					EntityTypeCode = generator.EntityTypeCode,
					EntityUid = generator.EntityUid,
					Item = new EntityStatus
					{
						Code = "draft",
						Name = "The Draft"
					}
				};

				var result = await handler.Handle(request, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.AffectedRows);

				var statuses = await generator.GetEntityStatuses(cancellationToken);

				Assert.AreEqual(2, statuses.Rows.Count);

				Assert.AreEqual(1, statuses.Rows[0].DisplayOrder);
				Assert.AreEqual("draft", statuses.Rows[0].Code);
				Assert.AreEqual("The Draft", statuses.Rows[0].Name);

				Assert.AreEqual(2, statuses.Rows[1].DisplayOrder);
				Assert.AreEqual("published", statuses.Rows[1].Code);
				Assert.AreEqual("Published", statuses.Rows[1].Name);
			}
		}
	}
}
