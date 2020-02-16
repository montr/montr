using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestClass]
	public class ExportClassifierListHandlerTests
	{
		[TestMethod]
		public async Task ExportClassifierList_Should_ReturnStream()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var classifierRepository = new DbClassifierRepository(dbContextFactory, classifierTypeService, null, null);
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var handler = new ExportClassifierListHandler(classifierRepository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await dbHelper.InsertType(HierarchyType.None, cancellationToken);

				for (var i = 0; i < 100; i++)
				{
					await dbHelper.InsertItem($"{i:D4}", null, cancellationToken);
				}

				// act
				var command = new ExportClassifierList
				{
					Request = new ClassifierSearchRequest
					{
						UserUid = Guid.NewGuid(),
						TypeCode = dbHelper.TypeCode
					}
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsNotNull(result.Stream);
				Assert.IsNotNull(result.FileName);
				Assert.IsNotNull(result.ContentType);

				// temp
				using (var fs = new FileStream(result.FileName, FileMode.OpenOrCreate))
				{
					result.Stream.CopyTo(fs);
				}
			}
		}
	}
}
