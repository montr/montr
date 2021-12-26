using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Commands;
using Montr.Core.Impl.CommandHandlers;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Core.Tests.Services;
using Montr.Data.Linq2Db;
using NUnit.Framework;

namespace Montr.Core.Tests.CommandHandlers
{
	[TestFixture]
	public class InsertEntityStatusHandlerTests
	{
		[Test]
		public async Task Execute_Should_InsertEntityStatus()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var generator = new CoreDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new InsertEntityStatusHandler(unitOfWorkFactory, dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange

				// act
				var request = new InsertEntityStatus
				{
					EntityTypeCode = generator.EntityTypeCode,
					EntityUid = generator.EntityUid,
					Item = new EntityStatus
					{
						Uid = Guid.NewGuid(),
						DisplayOrder = 12,
						Code = "draft",
						Name = "Draft"
					}
				};

				var result = await handler.Handle(request, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.AffectedRows);

				var statuses = await generator.GetEntityStatuses(cancellationToken);

				Assert.AreEqual(1, statuses.Rows.Count);
				Assert.AreEqual(12, statuses.Rows[0].DisplayOrder);
				Assert.AreEqual("draft", statuses.Rows[0].Code);
				Assert.AreEqual("Draft", statuses.Rows[0].Name);
			}
		}
	}
}
