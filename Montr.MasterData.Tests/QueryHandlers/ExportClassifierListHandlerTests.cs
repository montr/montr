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
			var repository = new ClassifierRepository(dbContextFactory);

			var handler = new ExportClassifierListHandler(repository);

			// act
			var command = new ExportClassifierList
			{
				UserUid = Guid.NewGuid(),
				Request = new ClassifierSearchRequest
				{
					CompanyUid = Guid.Parse("6465dd4c-8664-4433-ba6a-14effd40ebed"),
					ConfigCode = "okved2"
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
