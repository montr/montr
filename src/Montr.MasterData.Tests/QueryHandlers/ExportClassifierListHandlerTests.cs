using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;
using Montr.MasterData.Services.Implementations;
using Montr.MasterData.Services.QueryHandlers;
using Montr.MasterData.Tests.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services.Implementations;
using Moq;
using NUnit.Framework;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestFixture]
	public class ExportClassifierListHandlerTests
	{
		[Test]
		public async Task ExportClassifierList_Should_ReturnStream()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

			var fieldProviderRegistry = new DefaultFieldProviderRegistry();
			fieldProviderRegistry.AddFieldType(typeof(TextField));
			fieldProviderRegistry.AddFieldType(typeof(TextAreaField));

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

			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);

			var classifierRepository = new DbClassifierRepository<Classifier>(
				dbContextFactory, classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null);

			var classifierRepositoryFactoryMock = new Mock<INamedServiceFactory<IClassifierRepository>>();
			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(generator.TypeCode))
				.Returns(() => classifierRepository);

			var handler = new ExportClassifierListHandler(classifierRepositoryFactoryMock.Object);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.None, cancellationToken);

				for (var i = 0; i < 100; i++)
				{
					await generator.InsertItem($"{i:D4}", null, cancellationToken);
				}

				// act
				var command = new ExportClassifierList
				{
					TypeCode = generator.TypeCode
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Stream, Is.Not.Null);
				Assert.That(result.FileName, Is.Not.Null);
				Assert.That(result.ContentType, Is.Not.Null);

				// temp
				using (var fs = new FileStream(result.FileName, FileMode.OpenOrCreate))
				{
					await result.Stream.CopyToAsync(fs, cancellationToken);
				}
			}
		}
	}
}
