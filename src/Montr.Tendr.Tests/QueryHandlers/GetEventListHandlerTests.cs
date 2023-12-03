using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services.Implementations;
using Montr.Tendr.Queries;
using Montr.Tendr.QueryHandlers;
using NUnit.Framework;

namespace Montr.Tendr.Tests.QueryHandlers
{
	[TestFixture]
	public class GetEventListHandlerTests
	{
		[Test]
		public async Task GetEventList_ForNormalRequest_ReturnItems()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();

			var handler = new GetEventListHandler(dbContextFactory);

			// act
			var command = new GetEventList
			{
				UserUid = Guid.NewGuid(),
			};

			var result = await handler.Handle(command, cancellationToken);

			// assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Rows, Is.Not.Null);
		}
	}
}
