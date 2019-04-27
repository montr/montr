using System;
using System.IO;
using System.Linq;
using System.Text;
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
	public class ClosureTableHandlerTests
	{
		private readonly Guid UserUid = Guid.NewGuid();
		private readonly string TypeCode = "test_closure";
		private readonly string TreeCode = ClassifierTree.DefaultTreeCode;

		[TestMethod]
		public async Task InsertGroup_Should_BuildClosureTable()
		{
			// arrange
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var insertClassifierTypeHandler = new InsertClassifierTypeHandler(unitOfWorkFactory, dbContextFactory);
			var insertClassifierGroupHandler = new InsertClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var cancellationToken = new CancellationToken();

			using (var _ = unitOfWorkFactory.Create())
			{
				// act
				await InsertType(insertClassifierTypeHandler, cancellationToken);
				await InsertGroups(2, 3, null, null, insertClassifierGroupHandler, cancellationToken);

				// assert
				var closure = PrintClosure(dbContextFactory);

				Assert.AreEqual(File.ReadAllText("../../../Content/closure.2x3.txt"), closure);
			}
		}
		[TestMethod]
		public async Task DeleteGroup_Should_RebuildClosureTable()
		{
			// arrange
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var insertClassifierTypeHandler = new InsertClassifierTypeHandler(unitOfWorkFactory, dbContextFactory);
			var insertClassifierGroupHandler = new InsertClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var deleteClassifierGroupHandler = new DeleteClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var cancellationToken = new CancellationToken();

			using (var _ = unitOfWorkFactory.Create())
			{
				// act
				await InsertType(insertClassifierTypeHandler, cancellationToken);
				await InsertGroups(3, 3, null, null, insertClassifierGroupHandler, cancellationToken);

				// assert
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3.txt"), PrintClosure(dbContextFactory));

				// act & assert
				await DeleteGroup(deleteClassifierGroupHandler, dbContextFactory, cancellationToken, "1");
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3-1.txt"), PrintClosure(dbContextFactory));

				// act & assert
				await DeleteGroup(deleteClassifierGroupHandler, dbContextFactory, cancellationToken, "2.2");
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3-1-2.2.txt"), PrintClosure(dbContextFactory));

				// act & assert
				await DeleteGroup(deleteClassifierGroupHandler, dbContextFactory, cancellationToken, "3.1.2");
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3-1-2.2-3.1.2.txt"), PrintClosure(dbContextFactory));
			}
		}

		private async Task InsertType(
			InsertClassifierTypeHandler insertClassifierTypeHandler, CancellationToken cancellationToken)
		{
			await insertClassifierTypeHandler.Handle(new InsertClassifierType
			{
				CompanyUid = Constants.OperatorCompanyUid,
				UserUid = UserUid,
				Item = new ClassifierType
				{
					Code = TypeCode, Name = TypeCode,
					HierarchyType = HierarchyType.Groups
				}
			}, cancellationToken);
		}

		private async Task InsertGroups(int count, int depth, string parentCode, Guid? parentUid,
			InsertClassifierGroupHandler insertClassifierGroupHandler, CancellationToken cancellationToken)
		{
			for (var i = 1; i <= count; i++)
			{
				var code = parentCode != null ? $"{parentCode}.{i}" : $"{i}" ;

				var uid = await insertClassifierGroupHandler.Handle(new InsertClassifierGroup
				{
					CompanyUid = Constants.OperatorCompanyUid,
					UserUid = UserUid,
					TypeCode = TypeCode,
					TreeCode = TreeCode,
					Item = new ClassifierGroup { Code = code, Name = $"Class {code}", ParentUid = parentUid }
				}, cancellationToken);

				if (depth > 1)
				{
					await InsertGroups(count, depth - 1, code, uid, insertClassifierGroupHandler, cancellationToken);
				}
			}
		}

		private async Task DeleteGroup(
			DeleteClassifierGroupHandler deleteClassifierGroupHandler,
			DefaultDbContextFactory dbContextFactory,
			CancellationToken cancellationToken, string groupCode)
		{
			await deleteClassifierGroupHandler.Handle(new DeleteClassifierGroup
			{
				CompanyUid = Constants.OperatorCompanyUid,
				UserUid = UserUid,
				TypeCode = TypeCode,
				TreeCode = TreeCode,
				Uid = await FindGroup(dbContextFactory, groupCode)
			}, cancellationToken);
		}

		private async Task<Guid> FindGroup(IDbContextFactory dbContextFactory, string groupCode)
		{
			using (var db = dbContextFactory.Create())
			{
				var query = from g in db.GetTable<DbClassifierGroup>()
					join tree in db.GetTable<DbClassifierTree>() on g.TreeUid equals tree.Uid
					join type in db.GetTable<DbClassifierType>() on tree.TypeUid equals type.Uid
					where type.Code == TypeCode
						&& tree.Code == TreeCode
						&& g.Code == groupCode
					select g;

				var group = await query.SingleAsync();

				return group.Uid;
			}
		}

		private string PrintClosure(IDbContextFactory dbContextFactory)
		{
			using (var db = dbContextFactory.Create())
			{
				var print = from c in db.GetTable<DbClassifierClosure>()
					join parent in db.GetTable<DbClassifierGroup>() on c.ParentUid equals parent.Uid
					join child in db.GetTable<DbClassifierGroup>() on c.ChildUid equals child.Uid
					join tree in db.GetTable<DbClassifierTree>() on parent.TreeUid equals tree.Uid
					join type in db.GetTable<DbClassifierType>() on tree.TypeUid equals type.Uid
					where type.Code == TypeCode
					      && tree.Code == TreeCode
					orderby parent.Code, child.Code, c.Level
					select new { ParentCode = parent.Code, ChildCode = child.Code, c.Level };

				var sb = new StringBuilder();
				sb.AppendLine("Parent       Child        Level");
				foreach (var line in print)
				{
					sb.AppendLine($"{line.ParentCode,-12} {line.ChildCode,-12} {line.Level}");
				}

				return sb.ToString();
			}
		}
	}
}
