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
using Montr.Metadata.Models;

namespace Montr.MasterData.Tests.Services
{
	[TestClass]
	public class ClosureTableHandlerTests
	{
		private readonly Guid UserUid = Guid.NewGuid();
		private readonly string TypeCode = "test_closure";

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
				var root = await FindGroup(dbContextFactory, ClassifierGroup.DefaultRootCode);
				await InsertGroups(2, 3, root.Code, root.Uid, insertClassifierGroupHandler, cancellationToken);

				// assert
				var closure = PrintClosure(dbContextFactory);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.2x3.txt"), closure);
			}
		}

		[TestMethod]
		public async Task UpdateGroup_ShouldThrow_WhenCyclicDependencyDetected()
		{
			// arrange
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var insertClassifierTypeHandler = new InsertClassifierTypeHandler(unitOfWorkFactory, dbContextFactory);
			var insertClassifierGroupHandler = new InsertClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var updateClassifierGroupHandler = new UpdateClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var cancellationToken = new CancellationToken();

			using (var _ = unitOfWorkFactory.Create())
			{
				// act & assert
				await InsertType(insertClassifierTypeHandler, cancellationToken);
				await InsertGroups(3, 3, null, null, insertClassifierGroupHandler, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3.txt"), PrintClosure(dbContextFactory));

				// act & assert - cyclic dependency
				var result = await UpdateGroup("1.1", "1.1.1", updateClassifierGroupHandler, dbContextFactory, cancellationToken);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.IsNotNull(result.Errors);
				Assert.AreEqual("parentUid", result.Errors[0].Key);
				Assert.AreEqual("Cyclic dependency detected.", result.Errors[0].Messages[0]);
			}
		}

		[TestMethod]
		public async Task UpdateGroup_Should_RebuildClosureTable()
		{
			// arrange
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var insertClassifierTypeHandler = new InsertClassifierTypeHandler(unitOfWorkFactory, dbContextFactory);
			var insertClassifierGroupHandler = new InsertClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var updateClassifierGroupHandler = new UpdateClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var cancellationToken = new CancellationToken();

			using (var _ = unitOfWorkFactory.Create())
			{
				// act & assert
				await InsertType(insertClassifierTypeHandler, cancellationToken);
				await InsertGroups(3, 3, null, null, insertClassifierGroupHandler, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3.txt"), PrintClosure(dbContextFactory));

				// act & assert - from null to not null parent
				await UpdateGroup("1", "2.1", updateClassifierGroupHandler, dbContextFactory, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3~1to2.1.txt"), PrintClosure(dbContextFactory));

				// act & assert - from not null to null parent
				await UpdateGroup("2.2", null, updateClassifierGroupHandler, dbContextFactory, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3~1to2.1~2.2toRoot.txt"), PrintClosure(dbContextFactory));

				// act & assert - from not null to not null parent
				await UpdateGroup("3.3", "1.3", updateClassifierGroupHandler, dbContextFactory, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3~1to2.1~2.2toRoot~3.3to1.3.txt"), PrintClosure(dbContextFactory));
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
				// act & assert
				await InsertType(insertClassifierTypeHandler, cancellationToken);
				await InsertGroups(3, 3, null, null, insertClassifierGroupHandler, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3.txt"), PrintClosure(dbContextFactory));

				// act & assert
				await DeleteGroup("1", deleteClassifierGroupHandler, dbContextFactory, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3-1.txt"), PrintClosure(dbContextFactory));

				// act & assert
				await DeleteGroup("2.2", deleteClassifierGroupHandler, dbContextFactory, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3-1-2.2.txt"), PrintClosure(dbContextFactory));

				// act & assert
				await DeleteGroup("3.1.2", deleteClassifierGroupHandler, dbContextFactory, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3-1-2.2-3.1.2.txt"), PrintClosure(dbContextFactory));
			}
		}

		private async Task<ApiResult> InsertType(InsertClassifierTypeHandler insertClassifierTypeHandler, CancellationToken cancellationToken)
		{
			var result = await insertClassifierTypeHandler.Handle(new InsertClassifierType
			{
				CompanyUid = Constants.OperatorCompanyUid,
				UserUid = UserUid,
				Item = new ClassifierType
				{
					Code = TypeCode, Name = TypeCode,
					HierarchyType = HierarchyType.Groups
				}
			}, cancellationToken);

			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Success);

			return result;
		}

		private async Task InsertGroups(int count, int depth, string parentCode, Guid? parentUid,
			InsertClassifierGroupHandler insertClassifierGroupHandler, CancellationToken cancellationToken)
		{
			for (var i = 1; i <= count; i++)
			{
				var code = parentCode != null ? $"{parentCode}.{i}" : $"{i}" ;

				var result = await insertClassifierGroupHandler.Handle(new InsertClassifierGroup
				{
					CompanyUid = Constants.OperatorCompanyUid,
					UserUid = UserUid,
					TypeCode = TypeCode,
					// TreeCode = TreeCode,
					Item = new ClassifierGroup { Code = code, Name = $"Class {code}", ParentUid = parentUid }
				}, cancellationToken);

				if (depth > 1)
				{
					await InsertGroups(count, depth - 1, code, result.Uid, insertClassifierGroupHandler, cancellationToken);
				}
			}
		}

		private async Task<ApiResult> UpdateGroup(string groupCode, string newParentGroupCode,
			UpdateClassifierGroupHandler updateClassifierGroupHandler, DefaultDbContextFactory dbContextFactory, CancellationToken cancellationToken)
		{
			var dbGroup = await FindGroup(dbContextFactory, groupCode);

			var item = new ClassifierGroup
			{
				Uid = dbGroup.Uid,
				Code = dbGroup.Code,
				Name = dbGroup.Name
			};

			if (newParentGroupCode != null)
			{
				var dbParentGroup = await FindGroup(dbContextFactory, newParentGroupCode);
				item.ParentUid = dbParentGroup.Uid;
			}
			
			return await updateClassifierGroupHandler.Handle(new UpdateClassifierGroup
			{
				CompanyUid = Constants.OperatorCompanyUid,
				UserUid = UserUid,
				TypeCode = TypeCode,
				// TreeCode = TreeCode,
				Item = item
			}, cancellationToken);
		}

		private async Task DeleteGroup(string groupCode,
			DeleteClassifierGroupHandler deleteClassifierGroupHandler, IDbContextFactory dbContextFactory, CancellationToken cancellationToken)
		{
			var group = await FindGroup(dbContextFactory, groupCode);

			await deleteClassifierGroupHandler.Handle(new DeleteClassifierGroup
			{
				CompanyUid = Constants.OperatorCompanyUid,
				UserUid = UserUid,
				TypeCode = TypeCode,
				// TreeCode = TreeCode,
				Uid = group.Uid
			}, cancellationToken);
		}

		private async Task<DbClassifierGroup> FindGroup(IDbContextFactory dbContextFactory, string groupCode)
		{
			using (var db = dbContextFactory.Create())
			{
				var query = from g in db.GetTable<DbClassifierGroup>()
					// join tree in db.GetTable<DbClassifierTree>() on g.TreeUid equals tree.Uid
					join type in db.GetTable<DbClassifierType>() on g.TypeUid equals type.Uid
					where type.Code == TypeCode
						// && tree.Code == TreeCode
						&& g.Code == groupCode
					select g;

				return await query.SingleAsync();
			}
		}

		private string PrintClosure(IDbContextFactory dbContextFactory)
		{
			const int printColumnWidth = 16;

			using (var db = dbContextFactory.Create())
			{
				var print = from c in db.GetTable<DbClassifierClosure>()
					join parent in db.GetTable<DbClassifierGroup>() on c.ParentUid equals parent.Uid
					join child in db.GetTable<DbClassifierGroup>() on c.ChildUid equals child.Uid
					join type in db.GetTable<DbClassifierType>() on parent.TypeUid equals type.Uid
					where type.Code == TypeCode
					orderby parent.Code, child.Code, c.Level
					select new { ParentCode = parent.Code, ChildCode = child.Code, c.Level };

				var sb = new StringBuilder();
				sb.AppendLine($"{"Parent", -printColumnWidth} {"Child", -printColumnWidth} Level");
				foreach (var line in print)
				{
					sb.AppendLine($"{line.ParentCode, -printColumnWidth} {line.ChildCode, -printColumnWidth} {line.Level}");
				}

				return sb.ToString();
			}
		}
	}
}
