using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.CommandHandlers;
using Montr.Core.Commands;
using Montr.Core.Models;
using Montr.Core.Services.Impl;
using Montr.Core.Tests.Services;
using NUnit.Framework;

namespace Montr.Core.Tests.CommandHandlers
{
	[TestFixture]
	public class UpdateEntityStatusHandlerTests
	{
		[Test]
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
				var item1 = new EntityStatus { Uid = Guid.NewGuid(), DisplayOrder = 1, Code = "drafted", Name = "Draft" };
				var item2 = new EntityStatus { Uid = Guid.NewGuid(), DisplayOrder = 2, Code = "published", Name = "Published" };

				await generator.InsertEntityStatus(item1, cancellationToken);
				await generator.InsertEntityStatus(item2, cancellationToken);

				// act
				var request = new UpdateEntityStatus
				{
					EntityTypeCode = generator.EntityTypeCode,
					EntityUid = generator.EntityUid,
					Item = new EntityStatus
					{
						Uid = item1.Uid,
						Code = "draft",
						Name = "The Draft",
						DisplayOrder = 10
					}
				};

				var result = await handler.Handle(request, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.AffectedRows);

				var statuses = await generator.GetEntityStatuses(cancellationToken);

				Assert.AreEqual(2, statuses.Rows.Count);

				Assert.AreEqual(2, statuses.Rows[0].DisplayOrder);
				Assert.AreEqual("published", statuses.Rows[0].Code);
				Assert.AreEqual("Published", statuses.Rows[0].Name);

				Assert.AreEqual(10, statuses.Rows[1].DisplayOrder);
				Assert.AreEqual("draft", statuses.Rows[1].Code);
				Assert.AreEqual("The Draft", statuses.Rows[1].Name);
			}
		}
	}
}
