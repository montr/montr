using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
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
using Montr.Kompany.Models;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Services;
using Montr.Metadata.Impl.Entities;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;
using Moq;

namespace Montr.Kompany.Tests.CommandHandlers
{
	[TestClass]
	public class CreateCompanyHandlerTests
	{
		[TestMethod]
		public async Task CreateCompany_Should_CreateCompany()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();

			var fieldProviderRegistry = new DefaultFieldProviderRegistry();
			fieldProviderRegistry.AddFieldType(typeof(TextField));

			var dbFieldMetadataRepository = new DbFieldMetadataRepository(dbContextFactory, fieldProviderRegistry, new NewtonsoftJsonSerializer());
			var dbFieldDataRepository = new DbFieldDataRepository(dbContextFactory, fieldProviderRegistry);
			var dbNumberGenerator = new DbNumberGenerator(dbContextFactory, dateTimeProvider, new INumberTagProvider[0]);
			var documentRepository = new DbDocumentRepository(dbContextFactory, dbFieldMetadataRepository, dbFieldDataRepository, dbNumberGenerator);
			var jsonSerializer = new DefaultJsonSerializer();
			var auditLogService = new DbAuditLogService(dbContextFactory, jsonSerializer);

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

			var handler = new CreateCompanyHandler(unitOfWorkFactory,
				dbContextFactory, dateTimeProvider, metadataRepositoryMock.Object, dbFieldDataRepository, documentRepository, auditLogService);

			using (var _ = unitOfWorkFactory.Create())
			{
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
						.Where(x => x.EntityTypeCode == Document.EntityTypeCode && x.EntityUid == dbDocument.Uid)
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
