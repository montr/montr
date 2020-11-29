using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Tests.Services;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Moq;

namespace Montr.MasterData.Tests.CommandHandlers
{
	[TestClass]
	public class UpdateClassifierHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_UpdateClassifier()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);

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

			var classifierRepository = new DbClassifierRepository<Classifier>(unitOfWorkFactory,
				dbContextFactory, dateTimeProvider, classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null);

			var classifierRepositoryFactoryMock = new Mock<INamedServiceFactory<IClassifierRepository>>();
			classifierRepositoryFactoryMock.Setup(x => x.GetNamedOrDefaultService(It.IsAny<string>())).Returns(classifierRepository);

			var handler = new UpdateClassifierHandler(classifierRepositoryFactoryMock.Object);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.None, cancellationToken);
				var insertItem = await generator.InsertItem("001", null, cancellationToken);

				// act
				var command = new UpdateClassifier
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					Item = new Classifier
					{
						// ReSharper disable once PossibleInvalidOperationException
						Uid = insertItem.Uid.Value,
						Type = generator.TypeCode,
						Code = "001",
						Name = "Test Classifier",
						Fields = new FieldData
						{
							{ "test1", "value1" },
							{ "test2", "value2" },
							{ "test3", "value3" }
						}
					}
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.AffectedRows);
			}
		}
	}
}
