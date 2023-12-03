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
				Assert.That(result, Is.Not.Null);
				Assert.That(result.AffectedRows, Is.EqualTo(1));

				var statuses = await generator.GetEntityStatuses(cancellationToken);

				Assert.That(statuses.Rows.Count, Is.EqualTo(1));
				Assert.That(statuses.Rows[0].DisplayOrder, Is.EqualTo(12));
				Assert.That(statuses.Rows[0].Code, Is.EqualTo("draft"));
				Assert.That(statuses.Rows[0].Name, Is.EqualTo("Draft"));
			}
		}
	}
}
