using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services.Impl;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Montr.Tendr.Impl.QueryHandlers;
using Montr.Tendr.Queries;
using Moq;
using NUnit.Framework;

namespace Montr.Tendr.Tests.QueryHandlers
{
	[TestFixture]
	public class GetInvitationListHandlerTests
	{
		[Test]
		public async Task GetInvitationList_ForNormalRequest_ReturnItems()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

			var fieldProviderRegistry = new DefaultFieldProviderRegistry();
			fieldProviderRegistry.AddFieldType(typeof(TextField));
			var dbFieldDataRepository = new DbFieldDataRepository(dbContextFactory, fieldProviderRegistry);

			var metadataServiceMock = new Mock<IClassifierTypeMetadataService>();
			metadataServiceMock
				.Setup(x => x.GetMetadata(It.IsAny<ClassifierType>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(() => new FieldMetadata[]
				{
					new TextField { Key = "test1", Active = true, System = false },
					new TextField { Key = "test2", Active = true, System = false },
					new TextField { Key = "test3", Active = true, System = false }
				});

			var classifierRepository = new DbClassifierRepository<Classifier>(dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null);
			var handler = new GetInvitationListHandler(dbContextFactory, classifierRepository);

			// act
			var command = new GetInvitationList
			{
				UserUid = Guid.NewGuid(),
			};

			var result = await handler.Handle(command, cancellationToken);

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Rows);
		}
	}
}
