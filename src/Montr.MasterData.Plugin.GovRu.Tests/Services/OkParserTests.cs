using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Plugin.GovRu.Services;
using Montr.MasterData.Services;
using NUnit.Framework;

namespace Montr.MasterData.Plugin.GovRu.Tests.Services
{
	[TestFixture]
	public class OkParserTests
	{
		private async Task<ParseResult> Parse(IClassifierParser parser, string searchPattern)
		{
			var basePath = "../../../Content/";

			if (searchPattern.EndsWith(".zip"))
			{
				using (var stream = File.Open(Path.Combine(basePath, searchPattern) , FileMode.Open, FileAccess.Read))
				{
					using (var zip = new ZipArchive(stream))
					{
						foreach (var zipEntry in zip.Entries)
						{
							await parser.Parse(zipEntry.Open(), CancellationToken.None);
						}
					}
				}
			}
			else
			{
				foreach (var path in Directory.EnumerateFiles(basePath, searchPattern).Take(200))
				{
					using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
					{
						await parser.Parse(stream, CancellationToken.None);
					}
				}
			}

			return parser.GetResult();
		}

		[Test]
		[TestCase(false)]
		public async Task Parser_Should_ParseOkeiFile(bool dumpToDb)
		{
			// arrange
			var parser = new XmlOkeiParser();

			// act
			var result = await Parse(parser, "nsiOkei_*.xml");

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(561, result.Items.Count);
			Assert.AreEqual(28, result.Groups.Count);
			Assert.AreEqual(561, result.Links.Count);

			if (dumpToDb) await DumpToDb(result, "okei");
		}

		[Test]
		[TestCase(false)]
		public async Task Parser_Should_ParseOkved2File(bool dumpToDb)
		{
			// arrange
			var parser = new XmlOkved2Parser();

			// act
			var result = await Parse(parser, "nsiOkved2_*.xml");

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Items);
			Assert.AreEqual(2794, result.Items.Count);
			// Assert.AreEqual(2771, result.Items.Count(x => x.ParentCode != null));
			// Assert.AreEqual("66.19.7", result.Items.Single(x => x.Name == "Рейтинговая деятельность").Code);

			if (dumpToDb) await DumpToDb(result, "okved2");
		}

		[Test]
		[TestCase(false)]
		public async Task Parser_Should_ParseOkpd2File(bool dumpToDb)
		{
			// arrange
			var parser = new XmlOkpd2Parser();

			// act
			var result = await Parse(parser, "nsiOkpd2_*.xml");

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Items);
			Assert.AreEqual(3000, result.Items.Count);
			// Assert.AreEqual(57561, result.Items.Count);
			// Assert.AreEqual(19527, result.Items.Count(x => x.ParentCode != null));
			// Assert.AreEqual("66.19.7", result.Items.Single(x => x.Name == "Рейтинговая деятельность").Code);

			if (dumpToDb) await DumpToDb(result, "okpd2");
		}

		[Test]
		[TestCase(false)]
		public async Task Parser_Should_ParseOktmoXmlFile(bool dumpToDb)
		{
			// arrange
			var parser = new XmlOktmoParser();

			// act
			var result = await Parse(parser, "nsiOktmo_*.xml");

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Items);
			Assert.AreEqual(3000, result.Items.Count);
			// Assert.AreEqual(211185, result.Items.Count);

			if (dumpToDb) await DumpToDb(result, "oktmo");
		}

		[Test, Ignore("check it why ignored later")]
		[TestCase(false)]
		public async Task Parser_Should_ParseOktmoCsvFile(bool dumpToDb)
		{
			// arrange
			var parser = new CsvOktmoParser();

			// act
			// var result = await Parse(parser, "data-20190903t000000-structure-20150128t000000.zip");
			var result = await Parse(parser, "data-20190314t000000-structure-20150128t000000.csv");

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Items);
			Assert.AreEqual(200313, result.Items.Count);

			if (dumpToDb) await DumpToDb(result, "oktmo");
		}

		[Test]
		[TestCase(false)]
		public async Task Parser_Should_ParseOkvFile(bool dumpToDb)
		{
			// arrange
			var parser = new XmlOkvParser();

			// act
			var result = await Parse(parser, "nsiOkv_*.xml");

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Items);
			Assert.AreEqual(163, result.Items.Count);

			if (dumpToDb) await DumpToDb(result, "okv");
		}

		[Test]
		[TestCase(false)]
		public async Task Parser_Should_ParseOkopfFile(bool dumpToDb)
		{
			// arrange
			var parser = new XmlOkopfParser();

			// act
			var result = await Parse(parser, "nsiOkopf_*.xml");

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Items);
			Assert.AreEqual(223, result.Items.Count);

			if (dumpToDb) await DumpToDb(result, "okopf");
		}

		private static async Task DumpToDb(ParseResult result, string typeCode)
		{
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

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
