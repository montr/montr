using System;
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

			using (var scope = unitOfWorkFactory.Create())
			{
				// act
				var cancellationToken = new CancellationToken();

				await insertClassifierTypeHandler.Handle(new InsertClassifierType
				{
					CompanyUid = Constants.OperatorCompanyUid,
					UserUid = UserUid,
					Item = new ClassifierType { Code = TypeCode, Name = TypeCode, HierarchyType = HierarchyType.Groups }
				}, cancellationToken);

				await InsertGroups(3, 3, null, null, insertClassifierGroupHandler, cancellationToken);

				// assert
			}
		}

		private async Task InsertGroups(int count, int depth, string parentCode, Guid? parentUid,
			InsertClassifierGroupHandler insertClassifierGroupHandler, CancellationToken cancellationToken)
		{
			for (var i = 1; i <= count; i++)
			{
				var code = $"{parentCode}.{i}";

				Console.WriteLine(code);

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
	}
}
