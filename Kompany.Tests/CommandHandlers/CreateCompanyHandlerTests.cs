using System;
using System.Threading;
using System.Threading.Tasks;
using Dokumento.Implementation.Services;
using Kompany.Commands;
using Kompany.Implementation.CommandHandlers;
using Kompany.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Implementation.Services;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Kompany.Tests.CommandHandlers
{
	[TestClass]
	public class CreateCompanyHandlerTests
	{
		[TestMethod]
		public async Task CreateCompany_Should_CreateCompany()
		{
			// arrange
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var documentRepository = new DbDocumentRepository(dbContextFactory);
			var jsonSerializer = new DefaultJsonSerializer();
			var auditLogService = new DbAuditLogService(dbContextFactory, jsonSerializer);

			var handler = new CreateCompanyHandler(unitOfWorkFactory,
				dbContextFactory, dateTimeProvider, documentRepository, auditLogService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act
				var command = new CreateCompany
				{
					UserUid = Guid.NewGuid(),
					Company = new Company
					{
						ConfigCode = "company",
						Name = "Montr Inc."
					}
				};

				var uid = await handler.Handle(command, CancellationToken.None);

				// assert
				Assert.AreNotEqual(Guid.Empty, uid);
			}
		}
	}
}
