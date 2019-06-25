using System;
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
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Tests
{
	public class DbGenerator
	{
		private readonly Guid _userUid = Guid.NewGuid();
		private readonly string TypeCode = "test_closure";

		private readonly IDbContextFactory _dbContextFactory;
		private readonly InsertClassifierTypeHandler _insertClassifierTypeHandler;

		public DbGenerator(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
			_insertClassifierTypeHandler = new InsertClassifierTypeHandler(unitOfWorkFactory, dbContextFactory);
		}

		public async Task<DbClassifierGroup> InsertType(CancellationToken cancellationToken)
		{
			var result = await _insertClassifierTypeHandler.Handle(new InsertClassifierType
			{
				CompanyUid = Constants.OperatorCompanyUid,
				UserUid = _userUid,
				Item = new ClassifierType
				{
					Code = TypeCode,
					Name = TypeCode,
					HierarchyType = HierarchyType.Groups
				}
			}, cancellationToken);

			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Success);

			return await FindGroup(ClassifierGroup.DefaultRootCode);
		}

		public async Task<DbClassifierGroup> FindGroup(string groupCode)
		{
			using (var db = _dbContextFactory.Create())
			{
				var query = from g in db.GetTable<DbClassifierGroup>()
					join type in db.GetTable<DbClassifierType>() on g.TypeUid equals type.Uid
					where type.Code == TypeCode && g.Code == groupCode
					select g;

				return await query.SingleAsync();
			}
		}

		public async Task InsertGroups(int count, int depth, string parentCode, Guid? parentUid,
			InsertClassifierGroupHandler insertClassifierGroupHandler, CancellationToken cancellationToken)
		{
			for (var i = 1; i <= count; i++)
			{
				var code = parentCode != null ? $"{parentCode}.{i}" : $"{i}";

				var result = await insertClassifierGroupHandler.Handle(new InsertClassifierGroup
				{
					CompanyUid = Constants.OperatorCompanyUid,
					UserUid = _userUid,
					TypeCode = TypeCode,
					Item = new ClassifierGroup { Code = code, Name = $"Class {code}", ParentUid = parentUid }
				}, cancellationToken);

				Assert.IsNotNull(result);
				Assert.AreEqual(true, result.Success);

				if (depth > 1)
				{
					await InsertGroups(count, depth - 1, code, result.Uid, insertClassifierGroupHandler, cancellationToken);
				}
			}
		}

		public async Task<ApiResult> UpdateGroup(string groupCode, string newParentGroupCode,
			UpdateClassifierGroupHandler updateClassifierGroupHandler, CancellationToken cancellationToken)
		{
			var dbGroup = await FindGroup(groupCode);

			var item = new ClassifierGroup
			{
				Uid = dbGroup.Uid,
				Code = dbGroup.Code,
				Name = dbGroup.Name
			};

			if (newParentGroupCode != null)
			{
				var dbParentGroup = await FindGroup(newParentGroupCode);
				item.ParentUid = dbParentGroup.Uid;
			}

			return await updateClassifierGroupHandler.Handle(new UpdateClassifierGroup
			{
				CompanyUid = Constants.OperatorCompanyUid,
				UserUid = _userUid,
				TypeCode = TypeCode,
				Item = item
			}, cancellationToken);
		}

		public async Task DeleteGroup(string groupCode,
			DeleteClassifierGroupHandler deleteClassifierGroupHandler, CancellationToken cancellationToken)
		{
			var group = await FindGroup(groupCode);

			await deleteClassifierGroupHandler.Handle(new DeleteClassifierGroup
			{
				CompanyUid = Constants.OperatorCompanyUid,
				UserUid = _userUid,
				TypeCode = TypeCode,
				Uid = group.Uid
			}, cancellationToken);
		}

		public string PrintClosure()
		{
			const int printColumnWidth = 16;

			using (var db = _dbContextFactory.Create())
			{
				var print = from c in db.GetTable<DbClassifierClosure>()
							join parent in db.GetTable<DbClassifierGroup>() on c.ParentUid equals parent.Uid
							join child in db.GetTable<DbClassifierGroup>() on c.ChildUid equals child.Uid
							join type in db.GetTable<DbClassifierType>() on parent.TypeUid equals type.Uid
							where type.Code == TypeCode
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
