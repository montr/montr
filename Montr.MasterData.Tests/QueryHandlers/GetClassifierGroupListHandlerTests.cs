using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestClass]
	public class GetClassifierGroupListHandlerTests
	{
		[TestMethod]
		public async Task GetGroups_ForNullParent_ReturnItems()
		{
			// arrange
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService);

			// act
			var command = new GetClassifierGroupList
			{
				Request = new ClassifierGroupSearchRequest
				{
					CompanyUid = Constants.OperatorCompanyUid,
					TypeCode = "okei",
					TreeCode = "default"
				}
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(4, result.Count);
		}

		[TestMethod]
		public async Task GetGroups_ForNotNullParent_ReturnItems()
		{
			// arrange
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService);

			// act
			var command = new GetClassifierGroupList
			{
				Request = new ClassifierGroupSearchRequest
				{
					CompanyUid = Constants.OperatorCompanyUid,
					TypeCode = "okei",
					TreeCode = "default",
					ParentCode = "1"
				}
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(7, result.Count);
		}

		[TestMethod]
		public async Task GetItems_ForNullParent_ReturnItems()
		{
			// arrange
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService);

			// act
			var command = new GetClassifierGroupList
			{
				Request = new ClassifierGroupSearchRequest
				{
					CompanyUid = Constants.OperatorCompanyUid,
					TypeCode = "okved2"
				}
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(21, result.Count);
		}

		[TestMethod]
		public async Task GetItems_ForNotNullParent_ReturnItems()
		{
			// arrange
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService);

			// act
			var command = new GetClassifierGroupList
			{
				Request = new ClassifierGroupSearchRequest
				{
					CompanyUid = Constants.OperatorCompanyUid,
					TypeCode = "okved2",
					ParentCode = "F"
				}
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count);
		}


		[TestMethod]
		public async Task GetItems_ForBigGroups_ReturnNoMoreThan1000Items()
		{
			// arrange
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService);

			// act
			var command = new GetClassifierGroupList
			{
				Request = new ClassifierGroupSearchRequest
				{
					CompanyUid = Constants.OperatorCompanyUid,
					TypeCode = "oktmo"
				}
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Count <= 1000);
		}

		// todo: generate deep tree
		[TestMethod]
		public async Task GetGroups_WithFocusUid_ReturnAllChildrenOfParentGroups()
		{
			// arrange
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService);

			// act
			var command = new GetClassifierGroupList
			{
				Request = new ClassifierGroupSearchRequest
				{
					CompanyUid = Constants.OperatorCompanyUid,
					TypeCode = "okei",
					TreeCode = "default",
					FocusUid = Guid.Parse("fb9e31c0-3b53-4793-9de0-3b85f7b76b49") // 3.7. Экономические единицы
				}
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(4, result.Count);
			Assert.IsNull(result[0].Children);
			Assert.IsNull(result[1].Children);

			Assert.IsNotNull(result[2].Children); // 3. ЧЕТЫРЕХЗНАЧНЫЕ НАЦИОНАЛЬНЫЕ ЕДИНИЦЫ ИЗМЕНЕНИЯ, ВКЛЮЧЕННЫЕ В ОКЕИ
			Assert.AreEqual(result[2].Children.Count, 3);

			Assert.IsNull(result[3].Children);
		}
	}
}
