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
using Montr.MasterData.Plugin.GovRu.Services;
using Montr.MasterData.Services;

namespace Montr.MasterData.Plugin.GovRu.Tests.Services
{
	[TestClass]
	public class OkParserTests
	{
		private async Task<ParseResult> Parse(IClassifierParser parser, string searchPattern)
		{
			foreach (var path in Directory.EnumerateFiles("../../../Content/", searchPattern).Take(200))
			{
				using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
				{
					await parser.Parse(stream, CancellationToken.None);
				}
			}

			return parser.GetResult();
		}

		[TestMethod]
		public async Task Parser_Should_ParseOkeiFile()
		{
			// arrange
			var parser = new OkeiParser();

			// act
			var result = await Parse(parser, "nsiOkei_*.xml");

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Items);
			Assert.AreEqual(561, result.Items.Count);
			// Assert.AreEqual("728", result.Items.Single(x => x.Name == "Пачка").Code);

			await DumpToDb(result, "okei");
		}

		[TestMethod]
		public async Task Parser_Should_ParseOkved2File()
		{
			// arrange
			var parser = new Okved2Parser();

			// act
			var result = await Parse(parser, "nsiOkved2_*.xml");

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Items);
			Assert.AreEqual(2794, result.Items.Count);
			// Assert.AreEqual(2771, result.Items.Count(x => x.ParentCode != null));
			// Assert.AreEqual("66.19.7", result.Items.Single(x => x.Name == "Рейтинговая деятельность").Code);

			await DumpToDb(result, "okved2");
		}

		[TestMethod]
		public async Task Parser_Should_ParseOkpd2File()
		{
			// arrange
			var parser = new Okpd2Parser();

			// act
			var result = await Parse(parser, "nsiOkpd2_*.xml");

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Items);
			Assert.AreEqual(57561, result.Items.Count);
			// Assert.AreEqual(19527, result.Items.Count(x => x.ParentCode != null));
			// Assert.AreEqual("66.19.7", result.Items.Single(x => x.Name == "Рейтинговая деятельность").Code);

			await DumpToDb(result, "okpd2");
		}

		[TestMethod]
		public async Task Parser_Should_ParseOktmoFile()
		{
			// arrange
			var parser = new OktmoParser();

			// act
			var result = await Parse(parser, "nsiOktmo_*.xml");

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Items);
			Assert.AreEqual(211185, result.Items.Count);

			await DumpToDb(result, "oktmo");
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
