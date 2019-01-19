using System;
using System.Threading;
using System.Threading.Tasks;
using Kompany.Commands;
using Kompany.Implementation.CommandHandlers;
using Kompany.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Services;

namespace Kompany.Tests.CommandHandlers
{
	[TestClass]
	public class CreateCompanyHandlerTests
	{
		[TestMethod]
		public async Task CreateCompany_Should_CreateCompany()
		{
			// arrange
			TestHelper.InitDb();

			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory { Commitable = false };
			var handler = new CreateCompanyHandler(unitOfWorkFactory);

			// act
			var command = new CreateCompany
			{
				Company = new Company
				{
					ConfigCode = "company",
					Name = "Tendr Inc."
				}
			};
			var uid = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.AreNotEqual(Guid.Empty, uid);
		}
	}
}
