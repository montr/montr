using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Impl.Services;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Docs.Impl.Entities;
using Montr.Docs.Impl.Services;
using Montr.Docs.Models;
using Montr.Kompany.Commands;
using Montr.Kompany.Impl.CommandHandlers;
using Montr.Kompany.Impl.Services;
using Montr.Kompany.Models;
using Montr.Kompany.Registration.Services;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Tests.Services;
using Montr.Metadata.Impl.Entities;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Montr.Worker.Services;
using Moq;

namespace Montr.Kompany.Tests.CommandHandlers
{
	[TestClass]
	public class CreateCompanyHandlerTests
	{
		private static INamedServiceFactory<IClassifierRepository> CreateClassifierRepositoryFactory(IDbContextFactory dbContextFactory)
		{
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

			var fieldProviderRegistry = new DefaultFieldProviderRegistry();
			fieldProviderRegistry.AddFieldType(typeof(TextField));
			fieldProviderRegistry.AddFieldType(typeof(TextAreaField));
			var dbFieldDataRepository = new DbFieldDataRepository(dbContextFactory, fieldProviderRegistry);

			var metadataService = new ClassifierTypeMetadataService(new DbFieldMetadataRepository(
				dbContextFactory, fieldProviderRegistry, new NewtonsoftJsonSerializer()));

			var classifierRepository = new DbClassifierRepository<Classifier>(dbContextFactory,
				classifierTypeService, null, metadataService, dbFieldDataRepository, null);

			var numeratorRepository = new DbNumeratorRepository(dbContextFactory,
				classifierTypeService, null, metadataService, dbFieldDataRepository, null);

			var documentTypeRepository = new DbDocumentTypeRepository(dbContextFactory,
				classifierTypeService, null, metadataService, dbFieldDataRepository, null);

			var companyRepository = new DbCompanyRepository(dbContextFactory,
				classifierTypeService, null, metadataService, dbFieldDataRepository, null);

			var classifierRepositoryFactoryMock = new Mock<INamedServiceFactory<IClassifierRepository>>();

			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == MasterData.ClassifierTypeCode.Numerator)))
				.Returns(() => numeratorRepository);
			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == Docs.ClassifierTypeCode.DocumentType)))
				.Returns(() => documentTypeRepository);
			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == ClassifierTypeCode.Company)))
				.Returns(() => companyRepository);
			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name =>
					name != MasterData.ClassifierTypeCode.Numerator &&
					name != Docs.ClassifierTypeCode.DocumentType &&
					name != ClassifierTypeCode.Company)))
				.Returns(() => classifierRepository);

			return classifierRepositoryFactoryMock.Object;
		}

		[TestMethod]
		public async Task CreateCompany_Should_CreateCompany()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();

			var mediatorMock = new Mock<IPublisher>();

			var fieldProviderRegistry = new DefaultFieldProviderRegistry();
			fieldProviderRegistry.AddFieldType(typeof(TextField));

			// var dbFieldMetadataRepository = new DbFieldMetadataRepository(dbContextFactory, fieldProviderRegistry, new NewtonsoftJsonSerializer());
			var dbFieldDataRepository = new DbFieldDataRepository(dbContextFactory, fieldProviderRegistry);
			var classifierRepositoryFactory = CreateClassifierRepositoryFactory(dbContextFactory);
			var dbNumberGenerator = new DbNumberGenerator(dbContextFactory, classifierRepositoryFactory, dateTimeProvider, Array.Empty<INumberTagResolver>());
			var dbDocumentService = new DbDocumentService(dbContextFactory, dbNumberGenerator, mediatorMock.Object);
			var jsonSerializer = new DefaultJsonSerializer();
			var auditLogService = new DbAuditLogService(dbContextFactory, jsonSerializer);
			var classifierRegistrator = new DefaultClassifierRegistrator(NullLogger<DefaultClassifierRegistrator>.Instance, classifierRepositoryFactory);

			var metadataRepositoryMock = new Mock<IRepository<FieldMetadata>>();
			metadataRepositoryMock
				.Setup(x => x.Search(It.IsAny<MetadataSearchRequest>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(() => new SearchResult<FieldMetadata>
				{
					Rows = new FieldMetadata[]
					{
						new TextField { Key = "test1", Active = true, System = false },
						new TextField { Key = "test2", Active = true, System = false },
						new TextField { Key = "test3", Active = true, System = false }
					}
				});

			var backgroundJobManagerMock = new Mock<IBackgroundJobManager>();

			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);

			var handler = new CreateCompanyHandler(unitOfWorkFactory,
				dbContextFactory, dateTimeProvider, metadataRepositoryMock.Object,
				dbFieldDataRepository, classifierRepositoryFactory, dbDocumentService, auditLogService, backgroundJobManagerMock.Object);

			using (var _ = unitOfWorkFactory.Create())
			{
				await generator.EnsureClassifierTypeRegistered(Numerator.GetDefaultMetadata(), cancellationToken);
				await generator.EnsureClassifierTypeRegistered(DocumentType.GetDefaultMetadata(), cancellationToken);
				await generator.EnsureClassifierTypeRegistered(Company.GetDefaultMetadata(), cancellationToken);

				// register required document types
				foreach (var classifier in RegisterClassifiersStartupTask.GetClassifiers())
				{
					await classifierRegistrator.Register(classifier, cancellationToken);
				}

				// act
				var company = new Company
				{
					ConfigCode = "company",
					Name = "Montr Inc.",
					Fields = new FieldData
					{
						{ "test1", "value1" },
						{ "test2", "value2" },
						{ "test3", "value3" }
					}
				};

				var command = new CreateCompany
				{
					UserUid = Guid.NewGuid(),
					Item = company
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.IsNotNull(result.Uid);
				Assert.AreNotEqual(Guid.Empty, result.Uid);

				// assert registration request inserted
				DbDocument dbDocument;
				using (var db = dbContextFactory.Create())
				{
					dbDocument = await db.GetTable<DbDocument>()
						.Where(x => x.CompanyUid == result.Uid)
						.SingleAsync(cancellationToken);
				}

				Assert.IsNotNull(dbDocument);

				// assert registration request field data inserted
				IList<DbFieldData> fieldData;
				using (var db = dbContextFactory.Create())
				{
					fieldData = await db.GetTable<DbFieldData>()
						.Where(x => x.EntityTypeCode == Document.TypeCode && x.EntityUid == dbDocument.Uid)
						.ToListAsync(cancellationToken);
				}

				Assert.AreEqual(company.Fields.Count, fieldData.Count);
				Assert.AreEqual(company.Fields["test1"], fieldData.Single(x => x.Key == "test1").Value);
				Assert.AreEqual(company.Fields["test2"], fieldData.Single(x => x.Key == "test2").Value);
				Assert.AreEqual(company.Fields["test3"], fieldData.Single(x => x.Key == "test3").Value);
			}
		}
	}
}
