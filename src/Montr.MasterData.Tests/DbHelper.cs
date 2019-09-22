using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.Metadata.Models;

namespace Montr.MasterData.Tests
{
	public class DbHelper
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly GetClassifierTreeListHandler _getClassifierTreeListHandler;
		private readonly InsertClassifierTreeHandler _insertClassifierTreeTypeHandler;
		private readonly InsertClassifierTypeHandler _insertClassifierTypeHandler;
		private readonly InsertClassifierGroupHandler _insertClassifierGroupHandler;
		private readonly InsertClassifierHandler _insertClassifierHandler;
		private readonly UpdateClassifierGroupHandler _updateClassifierGroupHandler;
		private readonly DeleteClassifierGroupHandler _deleteClassifierGroupHandler;
		private readonly InsertClassifierLinkHandler _insertClassifierLinkHandler;
		private readonly GetClassifierLinkListHandler _getClassifierLinkListHandler;

		public DbHelper(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;

			var dateTimeProvider = new DefaultDateTimeProvider();
			var classifierTreeRepository = new DbClassifierTreeRepository(dbContextFactory);
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			_getClassifierTreeListHandler = new GetClassifierTreeListHandler(classifierTreeRepository);
			_insertClassifierTreeTypeHandler = new InsertClassifierTreeHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			_insertClassifierTypeHandler = new InsertClassifierTypeHandler(unitOfWorkFactory, dbContextFactory);
			_insertClassifierGroupHandler = new InsertClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			_insertClassifierHandler = new InsertClassifierHandler(unitOfWorkFactory, dbContextFactory, dateTimeProvider, classifierTypeService);
			_updateClassifierGroupHandler = new UpdateClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			_deleteClassifierGroupHandler = new DeleteClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			_insertClassifierLinkHandler = new InsertClassifierLinkHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			_getClassifierLinkListHandler = new GetClassifierLinkListHandler(dbContextFactory, classifierTypeService);
		}

		public string TypeCode { get; set; } = "test_closure";

		public Guid CompanyUid { get; set; } = Constants.OperatorCompanyUid;

		public Guid UserUid { get; set; } = Guid.NewGuid();

		public async Task<ApiResult> InsertType(HierarchyType hierarchyType, CancellationToken cancellationToken)
		{
			var result = await _insertClassifierTypeHandler.Handle(new InsertClassifierType
			{
				CompanyUid = CompanyUid,
				UserUid = UserUid,
				Item = new ClassifierType
				{
					Code = TypeCode,
					Name = TypeCode,
					HierarchyType = hierarchyType
				}
			}, cancellationToken);

			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Success);

			return result;
		}

		public async Task<ApiResult> InsertTree(string treeCode, CancellationToken cancellationToken)
		{
			var result = await _insertClassifierTreeTypeHandler.Handle(new InsertClassifierTree
			{
				CompanyUid = CompanyUid,
				UserUid = UserUid,
				TypeCode = TypeCode,
				Item = new ClassifierTree
				{
					Code = treeCode,
					Name = treeCode
				}
			}, cancellationToken);

			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Success);

			return result;
		}

		public async Task<SearchResult<ClassifierTree>> GetTrees(CancellationToken cancellationToken)
		{
			var result = await _getClassifierTreeListHandler.Handle(new GetClassifierTreeList
			{
				CompanyUid = CompanyUid,
				UserUid = UserUid,
				TypeCode = TypeCode,
			}, cancellationToken);

			Assert.IsNotNull(result);

			return result;
		}

		public async Task<DbClassifierTree> FindTree(string treeCode, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				var query = from tree in db.GetTable<DbClassifierTree>()
					join type in db.GetTable<DbClassifierType>() on tree.TypeUid equals type.Uid
					where type.Code == TypeCode && tree.Code == treeCode
							select tree;

				return await query.SingleAsync(cancellationToken);
			}
		}

		public async Task<DbClassifierGroup> FindGroup(string treeCode, string groupCode, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				var query = from g in db.GetTable<DbClassifierGroup>()
					join tree in db.GetTable<DbClassifierTree>() on g.TreeUid equals tree.Uid
					join type in db.GetTable<DbClassifierType>() on tree.TypeUid equals type.Uid
					where type.Code == TypeCode && tree.Code == treeCode && g.Code == groupCode
					select g;

				return await query.SingleAsync(cancellationToken);
			}
		}

		public async Task InsertGroups(Guid treeUid, int count, int depth, string parentCode, Guid? parentUid, CancellationToken cancellationToken)
		{
			for (var i = 1; i <= count; i++)
			{
				var code = parentCode != null ? $"{parentCode}.{i}" : $"{i}";

				var result = await InsertGroup(treeUid, code, parentUid, cancellationToken);

				if (depth > 1)
				{
					await InsertGroups(treeUid, count, depth - 1, code, result.Uid, cancellationToken);
				}
			}
		}

		public async Task<ApiResult> InsertGroup(Guid treeUid, string code, Guid? parentUid, CancellationToken cancellationToken)
		{
			var result = await _insertClassifierGroupHandler.Handle(new InsertClassifierGroup
			{
				CompanyUid = CompanyUid,
				UserUid = UserUid,
				TypeCode = TypeCode,
				TreeUid = treeUid,
				Item = new ClassifierGroup {Code = code, Name = $"Test Group {code}", ParentUid = parentUid}
			}, cancellationToken);

			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Success);

			return result;
		}

		public async Task<ApiResult> UpdateGroup(string treeCode, string groupCode, string newParentGroupCode,
			CancellationToken cancellationToken, bool assertResult = true)
		{
			var dbGroup = await FindGroup(treeCode, groupCode, cancellationToken);

			var item = new ClassifierGroup
			{
				Uid = dbGroup.Uid,
				Code = dbGroup.Code,
				Name = dbGroup.Name
			};

			if (newParentGroupCode != null)
			{
				var dbParentGroup = await FindGroup(treeCode, newParentGroupCode, cancellationToken);
				item.ParentUid = dbParentGroup.Uid;
			}

			var result = await _updateClassifierGroupHandler.Handle(new UpdateClassifierGroup
			{
				CompanyUid = CompanyUid,
				UserUid = UserUid,
				TypeCode = TypeCode,
				Item = item
			}, cancellationToken);

			if (assertResult)
			{
				Assert.IsNotNull(result);
				Assert.AreEqual(true, result.Success);
			}

			return result;
		}

		public async Task<ApiResult> DeleteGroup(string treeCode, string groupCode, CancellationToken cancellationToken)
		{
			var group = await FindGroup(treeCode, groupCode, cancellationToken);

			var result = await _deleteClassifierGroupHandler.Handle(new DeleteClassifierGroup
			{
				CompanyUid = CompanyUid,
				UserUid = UserUid,
				TypeCode = TypeCode,
				Uid = group.Uid
			}, cancellationToken);

			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Success);

			return result;
		}

		public async Task<ApiResult> InsertItem(string itemCode, CancellationToken cancellationToken)
		{
			var result = await _insertClassifierHandler.Handle(new InsertClassifier
			{
				UserUid = UserUid,
				CompanyUid = CompanyUid,
				TypeCode = TypeCode,
				Item = new Classifier
				{
					Code = itemCode,
					Name = itemCode + " - Test Classifier"
				}
			}, cancellationToken);

			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Success);

			return result;
		}

		public async Task<ApiResult> InsertLink(Guid? groupUid, Guid? itemUid, CancellationToken cancellationToken)
		{
			var result = await _insertClassifierLinkHandler.Handle(new InsertClassifierLink
			{
				UserUid = UserUid,
				CompanyUid = CompanyUid,
				TypeCode = TypeCode,
				// ReSharper disable once PossibleInvalidOperationException
				GroupUid = groupUid.Value,
				// ReSharper disable once PossibleInvalidOperationException
				ItemUid = itemUid.Value
			}, cancellationToken);

			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Success);

			return result;
		}

		public async Task<SearchResult<ClassifierLink>> GetLinks(Guid? groupUid, Guid? itemUid, CancellationToken cancellationToken)
		{
			return await _getClassifierLinkListHandler.Handle(new GetClassifierLinkList
			{
				CompanyUid = CompanyUid,
				UserUid = UserUid,
				TypeCode = TypeCode,
				GroupUid = groupUid,
				ItemUid = itemUid
			}, cancellationToken);
		}

		public string PrintClosure(string treeCode)
		{
			const int printColumnWidth = 16;

			using (var db = _dbContextFactory.Create())
			{
				var print = from c in db.GetTable<DbClassifierClosure>()
					join parent in db.GetTable<DbClassifierGroup>() on c.ParentUid equals parent.Uid
					join child in db.GetTable<DbClassifierGroup>() on c.ChildUid equals child.Uid
					join tree in db.GetTable<DbClassifierTree>() on parent.TreeUid equals tree.Uid
					join type in db.GetTable<DbClassifierType>() on tree.TypeUid equals type.Uid
					where type.Code == TypeCode && tree.Code == treeCode
					orderby parent.Code, child.Code, c.Level
					select new { ParentCode = parent.Code, ChildCode = child.Code, c.Level };

				var sb = new StringBuilder().AppendLine($"{"Parent",-printColumnWidth} {"Child",-printColumnWidth} Level");

				foreach (var line in print)
				{
					sb.AppendLine($"{line.ParentCode,-printColumnWidth} {line.ChildCode,-printColumnWidth} {line.Level}");
				}

				return sb.ToString();
			}
		}
	}
}
