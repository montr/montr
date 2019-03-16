using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;

namespace Montr.MasterData.Tests.Services
{
	[TestClass]
	public class OkParserTests
	{
		[TestMethod]
		public async Task Parser_Should_ParseOkeiFile()
		{
			// arrange
			var path = "../../../Content/nsiOkei_all_20190217_022439_001.xml";
			var parser = new OkeiParser();

			// act
			ParseResult result;
			using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
			{
				result = await parser.Parse(stream, CancellationToken.None);
			}

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Items);
			Assert.IsTrue(result.Items.Count > 550 && result.Items.Count < 600);
			Assert.AreEqual("728", result.Items.Single(x => x.Name == "Пачка").Code);

			await DumpToDb(result, "okei");
		}

		[TestMethod]
		public async Task Parser_Should_ParseOkved2File()
		{
			// arrange
			var path = "../../../Content/nsiOkved2_all_20190217_022436_001.xml";
			var parser = new Okved2Parser();

			// act
			ParseResult result;
			using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
			{
				result = await parser.Parse(stream, CancellationToken.None);
			}

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Items);
			Assert.IsTrue(result.Items.Count > 2500 && result.Items.Count < 3000);
			Assert.AreEqual(2771, result.Items.Count(x => x.ParentCode != null));
			Assert.AreEqual("66.19.7", result.Items.Single(x => x.Name == "Рейтинговая деятельность").Code);

			await DumpToDb(result, "okved2");
		}

		private static async Task DumpToDb(ParseResult result, string typeCode)
		{
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var handler = new ImportClassifierListHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			await handler.Handle(new ImportClassifierList
			{
				CompanyUid = Guid.Parse("6465dd4c-8664-4433-ba6a-14effd40ebed"),
				UserUid = Guid.NewGuid(),
				TypeCode = typeCode,
				Data = result
			}, CancellationToken.None);
		}
	}
}
