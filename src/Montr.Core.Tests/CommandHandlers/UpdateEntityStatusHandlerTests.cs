using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Commands;
using Montr.Core.Models;
using Montr.Core.Services.CommandHandlers;
using Montr.Core.Services.Implementations;
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
				Assert.That(result, Is.Not.Null);
				Assert.That(result.AffectedRows, Is.EqualTo(1));

				var statuses = await generator.GetEntityStatuses(cancellationToken);

				Assert.That(statuses.Rows.Count, Is.EqualTo(2));

				Assert.That(statuses.Rows[0].DisplayOrder, Is.EqualTo(2));
				Assert.That(statuses.Rows[0].Code, Is.EqualTo("published"));
				Assert.That(statuses.Rows[0].Name, Is.EqualTo("Published"));

				Assert.That(statuses.Rows[1].DisplayOrder, Is.EqualTo(10));
				Assert.That(statuses.Rows[1].Code, Is.EqualTo("draft"));
				Assert.That(statuses.Rows[1].Name, Is.EqualTo("The Draft"));
			}
		}
	}
}
