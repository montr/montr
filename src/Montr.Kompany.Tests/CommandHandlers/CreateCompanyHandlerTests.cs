using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Montr.Core.Impl.Services;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Docs.Impl.Entities;
using Montr.Docs.Impl.Services;
using Montr.Docs.Models;
using Montr.Idx.Models;
using Montr.Kompany.Commands;
using Montr.Kompany.Impl.CommandHandlers;
using Montr.Kompany.Models;
using Montr.Kompany.Registration.Services;
using Montr.Kompany.Tests.Services;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Tests.Services;
using Montr.Metadata.Impl.Entities;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Montr.Worker.Services;
using Moq;
using NUnit.Framework;

namespace Montr.Kompany.Tests.CommandHandlers
{
	[TestFixture]
	public class CreateCompanyHandlerTests
	{
		[Test]
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
			var classifierRepositoryFactory = CompanyMockHelper.CreateClassifierRepositoryFactory(dbContextFactory);
			var dbNumberGenerator = new DbNumberGenerator(dbContextFactory, classifierRepositoryFactory, dateTimeProvider, Array.Empty<INumberTagResolver>());
			var dbDocumentService = new DbDocumentService(dbContextFactory, null, dbNumberGenerator, mediatorMock.Object, null, null);
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
						new TextField { Key = "test1", Active = true },
						new TextField { Key = "test2", Active = true },
						new TextField { Key = "test3", Active = true }
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
				await generator.EnsureClassifierTypeRegistered(User.GetDefaultMetadata(), cancellationToken);

				// register required document types
				foreach (var classifier in RegisterClassifiersStartupTask.GetClassifiers())
				{
					await classifierRegistrator.Register(classifier, cancellationToken);
				}

				var userRepository = classifierRepositoryFactory.GetNamedOrDefaultService(Idx.ClassifierTypeCode.User);

				var user = await userRepository.Insert(new User { Name = "User 1" }, cancellationToken);

				Assert.IsTrue(user.Success);
				Assert.IsNotNull(user.Uid);

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

				var request = new CreateCompany { Item = company, UserUid = user.Uid };
				var result = await handler.Handle(request, cancellationToken);

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
						.Where(x => x.EntityTypeCode == Docs.EntityTypeCode.Document && x.EntityUid == dbDocument.Uid)
						.ToListAsync(cancellationToken);
				}

				// todo: restore after registration rework
				// Assert.AreEqual(company.Fields.Count, fieldData.Count);
				Assert.AreEqual(company.Fields["test1"], fieldData.Single(x => x.Key == "test1").Value);
				Assert.AreEqual(company.Fields["test2"], fieldData.Single(x => x.Key == "test2").Value);
				Assert.AreEqual(company.Fields["test3"], fieldData.Single(x => x.Key == "test3").Value);
			}
		}
	}
}
