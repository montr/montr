using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Metadata.Services.Implementations;
using Montr.Tasks.Queries;
using Montr.Tasks.Services.QueryHandlers;
using Moq;
using NUnit.Framework;

namespace Montr.Tasks.Tests.QueryHandlers
{
	[TestFixture]
	public class GetTaskSearchCriteriaHandlerTests
	{
		[Test]
		public async Task Handle_GetTaskSearchCriteria_ShouldReturnMetadata()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var serviceProviderMock = new Mock<IServiceProvider>();
			var dataAnnotationMetadataProvider = new DefaultDataAnnotationMetadataProvider(serviceProviderMock.Object);
			var handler = new GetTaskSearchCriteriaHandler(dataAnnotationMetadataProvider);

			// act
			var result = await handler.Handle(new GetTaskSearchMetadata(), cancellationToken);

			// assert
			Assert.That(result, Is.Not.Null);

			Assert.That(result.Fields?.Count, Is.EqualTo(8));
			Assert.That(result.Columns?.Count, Is.EqualTo(9));

			Assert.That(result.Toolbar, Is.Null);
			Assert.That(result.Panels, Is.Null);
			Assert.That(result.Panes, Is.Null);
		}
	}
}
