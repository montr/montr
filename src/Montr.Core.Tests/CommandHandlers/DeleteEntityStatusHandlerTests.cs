using System;
using System.Collections.Generic;
using System.Linq;
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
	public class DeleteEntityStatusHandlerTests
	{
		[Test]
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
				var uids = new List<Guid>();
				for (var i = 0; i < 42; i++)
				{
					var insertResult = await generator.InsertEntityStatus(new EntityStatus
					{
						Uid = Guid.NewGuid(),
						Code = "status_code_" + i,
						Name = "Status Name " + i
					}, cancellationToken);

					// ReSharper disable once PossibleInvalidOperationException
					uids.Add(insertResult.Uid.Value);
				}

				// act
				var request = new DeleteEntityStatus
				{
					EntityTypeCode = generator.EntityTypeCode,
					EntityUid = generator.EntityUid,
					Uids = uids.Take(3).ToList()
				};

				var result = await handler.Handle(request, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(3, result.AffectedRows);

				var statuses = await generator.GetEntityStatuses(cancellationToken);

				Assert.AreEqual(39, statuses.Rows.Count);

				foreach (var uid in request.Uids)
				{
					Assert.IsNull(statuses.Rows.FirstOrDefault(x => x.Uid == uid));
				}
			}
		}
	}
}
