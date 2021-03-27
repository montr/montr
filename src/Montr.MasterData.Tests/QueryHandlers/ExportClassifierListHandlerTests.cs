using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;
using Montr.MasterData.Tests.Services;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Moq;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestClass]
	public class ExportClassifierListHandlerTests
	{
		[TestMethod]
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
					Request = new ClassifierSearchRequest
					{
						UserUid = Guid.NewGuid(),
						TypeCode = generator.TypeCode
					}
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsNotNull(result.Stream);
				Assert.IsNotNull(result.FileName);
				Assert.IsNotNull(result.ContentType);

				// temp
				using (var fs = new FileStream(result.FileName, FileMode.OpenOrCreate))
				{
					await result.Stream.CopyToAsync(fs, cancellationToken);
				}
			}
		}
	}
}
