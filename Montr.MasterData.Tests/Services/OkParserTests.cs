using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Entities;
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
			Assert.AreEqual("66.19.7", result.Items.Single(x => x.Name == "Рейтинговая деятельность").Code);

			// await DumpToDb(result, "okved2");
		}

		private static async Task DumpToDb(ParseResult result, string typeCode, string treeCode = "default")
		{
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);

			var handler = new InsertClassifierHandler(unitOfWorkFactory,
				dbContextFactory, dateTimeProvider, classifierTypeRepository);

			var companyUid = Guid.Parse("6465dd4c-8664-4433-ba6a-14effd40ebed");
			var userUid = Guid.NewGuid();

			var types = await classifierTypeRepository.Search(
				new ClassifierTypeSearchRequest
				{
					CompanyUid = companyUid,
					UserUid = userUid,
					Code = typeCode,
					/*PageNo = 1,
					PageSize = 1*/
				}, CancellationToken.None);

			var dbType = types.Rows.Single();

			// todo: build DAG for groups and items

			using (var scope = unitOfWorkFactory.Create())
			{
				using (var db = dbContextFactory.Create())
				{
					var dbTree = db.GetTable<DbClassifierTree>()
						.SingleOrDefault(x =>
							x.CompanyUid == companyUid &&
							x.TypeUid == dbType.Uid &&
							x.Code == treeCode);

					if (dbTree == null)
					{
						dbTree = new DbClassifierTree
						{
							// Name = treeCode,
							Uid = Guid.NewGuid(),
							Code = treeCode,
							CompanyUid = companyUid,
							TypeUid = dbType.Uid,
						};

						await db.InsertAsync(dbTree);
					}

					foreach (var group in result.Groups)
					{
						var dbGroup = new DbClassifierGroup
						{
							Uid = Guid.NewGuid(),
							CompanyUid = companyUid,
							Code = group.Code,
							Name = group.Name,
							TreeUid = dbTree.Uid,
							TypeUid = dbType.Uid
						};

						if (group.ParentCode != null)
						{
							var parentGroup = db.GetTable<DbClassifierGroup>()
								.Single(x =>
									x.CompanyUid == companyUid &&
									x.TypeUid == dbType.Uid &&
									x.TreeUid == dbTree.Uid &&
									x.Code == group.ParentCode);

							dbGroup.ParentUid = parentGroup.Uid;
						}

						await db.InsertAsync(dbGroup);
					}
				}

				// foreach (var classifier in result.Items)
				{
					var command = new InsertClassifier
					{
						UserUid = userUid,
						CompanyUid = companyUid,
						TypeCode = typeCode,
						Items = result.Items
					};

					await handler.Handle(command, CancellationToken.None);
				}

				using (var db = dbContextFactory.Create())
				{
					foreach (var itemInGroup in result.Links)
					{
						var dbGroup = db.GetTable<DbClassifierGroup>()
							.Single(x =>
								x.CompanyUid == companyUid &&
								x.TypeUid == dbType.Uid &&
								x.Code == itemInGroup.GroupCode);

						var dbItem = db.GetTable<DbClassifier>()
							.Single(x =>
								x.CompanyUid == companyUid &&
								x.TypeUid == dbType.Uid &&
								x.Code == itemInGroup.ItemCode);

						await db.InsertAsync(new DbClassifierLink
						{
							GroupUid = dbGroup.Uid,
							ItemUid = dbItem.Uid
						});
					}
				}

				scope.Commit();
			}
		}
	}
}
