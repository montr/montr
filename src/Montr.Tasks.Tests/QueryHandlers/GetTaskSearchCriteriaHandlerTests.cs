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
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Fields);
			Assert.AreEqual(result.Fields.Count, 8);
		}
	}
}
