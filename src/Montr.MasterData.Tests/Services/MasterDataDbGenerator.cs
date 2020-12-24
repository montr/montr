using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Logging;
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
using Montr.MasterData.Services;
using Montr.Metadata.Impl.Services;
using Moq;

namespace Montr.MasterData.Tests.Services
{
	public class MasterDataDbGenerator
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly DbClassifierTypeService _classifierTypeService;
		private readonly DefaultClassifierTypeRegistrator _classifierTypeRegistrator;
		private readonly GetClassifierTreeListHandler _getClassifierTreeListHandler;
		private readonly InsertClassifierTreeHandler _insertClassifierTreeTypeHandler;
		private readonly InsertClassifierTypeHandler _insertClassifierTypeHandler;
		private readonly InsertClassifierGroupHandler _insertClassifierGroupHandler;
		private readonly InsertClassifierHandler _insertClassifierHandler;
		private readonly UpdateClassifierGroupHandler _updateClassifierGroupHandler;
		private readonly DeleteClassifierGroupHandler _deleteClassifierGroupHandler;
		private readonly InsertClassifierLinkHandler _insertClassifierLinkHandler;
		private readonly GetClassifierLinkListHandler _getClassifierLinkListHandler;

		public MasterDataDbGenerator(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;

			var dateTimeProvider = new DefaultDateTimeProvider();
			var classifierTreeRepository = new DbClassifierTreeRepository(dbContextFactory);
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var dbFieldMetadataRepository = new DbFieldMetadataRepository(dbContextFactory, null, new NewtonsoftJsonSerializer());
			var dbFieldDataRepository = new DbFieldDataRepository(dbContextFactory, null);

			_classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			_getClassifierTreeListHandler = new GetClassifierTreeListHandler(classifierTreeRepository);
			_insertClassifierTreeTypeHandler = new InsertClassifierTreeHandler(unitOfWorkFactory, dbContextFactory, _classifierTypeService);
			_insertClassifierTypeHandler = new InsertClassifierTypeHandler(unitOfWorkFactory, _classifierTypeService);
			_insertClassifierGroupHandler = new InsertClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, _classifierTypeService);

			_classifierTypeRegistrator = new DefaultClassifierTypeRegistrator(new Mock<ILogger<DefaultClassifierTypeRegistrator>>().Object,
				unitOfWorkFactory, _classifierTypeService, new DbFieldMetadataService(dbContextFactory, new NewtonsoftJsonSerializer()));

			var classifierTypeMetadataService = new ClassifierTypeMetadataService(dbFieldMetadataRepository);
			var classifierTreeService = new DefaultClassifierTreeService(classifierTreeRepository);
			var dbNumberGenerator = new DbNumberGenerator(dbContextFactory, null, dateTimeProvider, null);

			var classifierRepositoryFactory = new ClassifierRepositoryFactory(new DbClassifierRepository<Classifier>(
				unitOfWorkFactory, dbContextFactory, dateTimeProvider, _classifierTypeService, classifierTreeService,
				classifierTypeMetadataService, dbFieldDataRepository, dbNumberGenerator));

			_insertClassifierHandler = new InsertClassifierHandler(classifierRepositoryFactory);
			_updateClassifierGroupHandler = new UpdateClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, _classifierTypeService);
			_deleteClassifierGroupHandler = new DeleteClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, _classifierTypeService);
			_insertClassifierLinkHandler = new InsertClassifierLinkHandler(unitOfWorkFactory, dbContextFactory, _classifierTypeService);
			_getClassifierLinkListHandler = new GetClassifierLinkListHandler(dbContextFactory, _classifierTypeService);
		}

		public string TypeCode { get; set; } = "test_closure";

		public Guid CompanyUid { get; set; } = Constants.OperatorCompanyUid;

		public Guid UserUid { get; set; } = Guid.NewGuid();

		public async Task<ApiResult> InsertType(HierarchyType hierarchyType, CancellationToken cancellationToken)
		{
			var result = await _insertClassifierTypeHandler.Handle(new InsertClassifierType
			{
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
				Item = new ClassifierGroup { Code = code, Name = $"Test Group {code}", ParentUid = parentUid }
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

		public async Task<ApiResult> InsertItem(string itemCode, Guid? parentUid, CancellationToken cancellationToken)
		{
			var result = await _insertClassifierHandler.Handle(new InsertClassifier
			{
				UserUid = UserUid,
				CompanyUid = CompanyUid,
				Item = new Classifier
				{
					Type = TypeCode,
					Code = itemCode,
					Name = itemCode + " - Test Classifier",
					ParentUid = parentUid
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

				var sb = new StringBuilder().AppendLine(
					$"{"Parent",-printColumnWidth} {"Child",-printColumnWidth} Level");

				foreach (var line in print)
				{
					sb.AppendLine(
						$"{line.ParentCode,-printColumnWidth} {line.ChildCode,-printColumnWidth} {line.Level}");
				}

				return sb.ToString();
			}
		}

		public async Task<ClassifierType> EnsureNumeratorTypeRegistered(CancellationToken cancellationToken)
		{
			var numeratorType = RegisterClassifierTypeStartupTask.GetNumeratorType();

			await _classifierTypeRegistrator.Register(numeratorType.Item, numeratorType.Fields, cancellationToken);

			return await _classifierTypeService.Get(DbNumeratorRepository.TypeCode, cancellationToken);
		}

		// todo: use numerator repository (?)
		public async Task<ApiResult> InsertNumerator(Numerator numerator, GenerateNumberRequest request, CancellationToken cancellationToken)
		{
			var type = await EnsureNumeratorTypeRegistered(cancellationToken);

			var numeratorUid = Guid.NewGuid();

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbClassifier>()
					.Value(x => x.Uid, numeratorUid)
					.Value(x => x.TypeUid, type.Uid)
					.Value(x => x.StatusCode, ClassifierStatusCode.Active)
					.Value(x => x.Code, numeratorUid.ToString().Substring(0, 32))
					.Value(x => x.Name, numerator.Name ?? "Test numerator")
					.Value(x => x.IsActive, true)
					.Value(x => x.IsSystem, false)
					.InsertAsync(cancellationToken);

				await db.GetTable<DbNumerator>()
					.Value(x => x.Uid, numeratorUid)
					.Value(x => x.EntityTypeCode, request.EntityTypeCode)
					.Value(x => x.Pattern, numerator.Pattern ?? Numerator.DefaultPattern)
					.Value(x => x.Periodicity, numerator.Periodicity.ToString())
					.Value(x => x.KeyTags, numerator.KeyTags != null ? string.Join(DbNumerator.KeyTagsSeparator, numerator.KeyTags) : null)
					.InsertAsync(cancellationToken);

				await db.GetTable<DbNumeratorEntity>()
					.Value(x => x.IsAutoNumbering, true)
					.Value(x => x.NumeratorUid, numeratorUid)
					.Value(x => x.EntityUid, request.EntityTypeUid)
					.InsertAsync(cancellationToken);

				return new ApiResult { Uid = numeratorUid };
			}
		}

		private class ClassifierRepositoryFactory : INamedServiceFactory<IClassifierRepository>
		{
			private readonly IClassifierRepository _defaultClassifierRepository;

			public ClassifierRepositoryFactory(IClassifierRepository defaultClassifierRepository)
			{
				_defaultClassifierRepository = defaultClassifierRepository;
			}

			public IEnumerable<string> GetNames()
			{
				throw new NotImplementedException();
			}

			public IClassifierRepository GetService(string name)
			{
				throw new NotImplementedException();
			}

			public IClassifierRepository GetRequiredService(string name)
			{
				throw new NotImplementedException();
			}

			public IClassifierRepository GetNamedOrDefaultService(string name)
			{
				return _defaultClassifierRepository;
			}
		}
	}
}
