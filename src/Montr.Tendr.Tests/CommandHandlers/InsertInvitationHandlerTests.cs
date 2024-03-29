﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Services.Implementations;
using Montr.MasterData.Tests.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services.Implementations;
using Montr.Tendr.CommandHandlers;
using Montr.Tendr.Commands;
using Montr.Tendr.Models;
using Moq;
using NUnit.Framework;

namespace Montr.Tendr.Tests.CommandHandlers
{
	[TestFixture]
	public class InsertInvitationHandlerTests
	{
		[Test]
		public async Task Handle_NormalValues_InsertInvitation()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

			var fieldProviderRegistry = new DefaultFieldProviderRegistry();
			fieldProviderRegistry.AddFieldType(typeof(TextField));
			var dbFieldDataRepository = new DbFieldDataRepository(NullLogger<DbFieldDataRepository>.Instance, dbContextFactory, fieldProviderRegistry);

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
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);

			var classifierRepositoryFactoryMock = new Mock<INamedServiceFactory<IClassifierRepository>>();
			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.IsAny<string>()))
				.Returns(() => classifierRepository);

			var handler = new InsertInvitationHandler(unitOfWorkFactory, dbContextFactory, classifierRepositoryFactoryMock.Object);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act
				var command = new InsertInvitation
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					EventUid = Guid.Parse("436c290c-37b2-11e9-88fe-00ff279ba9e1"),
					Items = new []
					{
						new Invitation { CounterpartyUid = Guid.Parse("1bef28d6-2255-416c-a706-008e0c179508") }
					}
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Success);
				Assert.That(result.AffectedRows, Is.EqualTo(1));
			}
		}
	}
}
