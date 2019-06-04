using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
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
					ParentUid = null // yeah, we test this
					// TreeCode = "default"
				}
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(ClassifierGroup.DefaultRootCode, result[0].Code);
		}

		[TestMethod]
		public async Task GetGroups_ForNotNullParent_ReturnItems()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService);
			var parentGroup = await FindClassifierGroup(dbContextFactory, "okei", "1", cancellationToken);

			// act
			var command = new GetClassifierGroupList
			{
				Request = new ClassifierGroupSearchRequest
				{
					CompanyUid = Constants.OperatorCompanyUid,
					TypeCode = "okei",
					// TreeCode = "default",
					ParentUid = parentGroup.Uid
				}
			};

			var result = await handler.Handle(command, cancellationToken);

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(7, result.Count);
		}

		private async Task<DbClassifierGroup> FindClassifierGroup(IDbContextFactory dbContextFactory, string typeCode, string code, CancellationToken cancellationToken)
		{
			using (var db = dbContextFactory.Create())
			{
				return await (
						from type in db.GetTable<DbClassifierType>().Where(x => x.Code == typeCode)
						/*join tree in db.GetTable<DbClassifierTree>()
								.Where(x => x.Code == "default")
							on type.Uid equals tree.TypeUid*/
						join @group in db.GetTable<DbClassifierGroup>().Where(x => x.Code == code)
							on type.Uid equals @group.TypeUid
						select @group)
					.SingleAsync(cancellationToken);
			}
		}

		[TestMethod, Ignore]
		public async Task GetGroups_WhenAutoExpandSingleChildRequested_ShouldReturnExpanded()
		{
			await Task.FromException<NotImplementedException>(new NotImplementedException());
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
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService);

			DbClassifier parentItem;
			using (var db = dbContextFactory.Create())
			{
				parentItem = await (
						from type in db.GetTable<DbClassifierType>()
							.Where(x => x.Code == "okved2")
						join item in db.GetTable<DbClassifier>()
								.Where(x => x.Code == "F")
							on type.Uid equals item.TypeUid
						select item)
					.SingleAsync(cancellationToken);
			}

			// act
			var command = new GetClassifierGroupList
			{
				Request = new ClassifierGroupSearchRequest
				{
					CompanyUid = Constants.OperatorCompanyUid,
					TypeCode = "okved2",
					ParentUid = parentItem.Uid // Guid.NewGuid() // ParentCode = "F"
				}
			};

			var result = await handler.Handle(command, cancellationToken);

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count);
		}

		[TestMethod, Ignore]
		public async Task GetItems_WhenAutoExpandSingleChildRequested_ShouldReturnExpanded()
		{
			await Task.FromException<NotImplementedException>(new NotImplementedException());
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
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService);
			var focusGroup = await FindClassifierGroup(dbContextFactory, "okei", "3.7", cancellationToken);

			// act
			var command = new GetClassifierGroupList
			{
				Request = new ClassifierGroupSearchRequest
				{
					CompanyUid = Constants.OperatorCompanyUid,
					TypeCode = "okei",
					// TreeCode = "default",
					FocusUid = focusGroup.Uid // 3.7. Экономические единицы
				}
			};

			var result = await handler.Handle(command, cancellationToken);

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
