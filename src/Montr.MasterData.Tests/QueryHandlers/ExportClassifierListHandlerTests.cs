using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestClass]
	public class ExportClassifierListHandlerTests
	{
		[TestMethod]
		public async Task ExportClassifierList_Should_ReturnStream()
		{
			// arrange
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			var classifierRepository = new DbClassifierRepository(dbContextFactory, classifierTypeService, null);

			var handler = new ExportClassifierListHandler(classifierRepository);

			// act
			var command = new ExportClassifierList
			{
				Request = new ClassifierSearchRequest
				{
					CompanyUid = Constants.OperatorCompanyUid,
					UserUid = Guid.NewGuid(),
					TypeCode = "okved2"
				}
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Stream);
			Assert.IsNotNull(result.FileName);
			Assert.IsNotNull(result.ContentType);

			// temp
			using (var fs = new FileStream(result.FileName, FileMode.OpenOrCreate))
			{
				result.Stream.CopyTo(fs);
			}
		}
	}
}
