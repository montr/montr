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
using Montr.MasterData.Services;
using Montr.MasterData.Tests.Services;
using Moq;

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

			var ctpMock = new Mock<INamedServiceFactory<IClassifierTypeProvider>>();
			ctpMock.Setup(x => x.GetNamedOrDefaultService(It.IsAny<string>()))
				.Returns(new ClassifierTypeProvider(dbContextFactory, null, null));

			var classifierRepository = new DbClassifierRepository(classifierTypeService, ctpMock.Object);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new ExportClassifierListHandler(classifierRepository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.None, cancellationToken);

				for (var i = 0; i < 100; i++)
				{
					await generator.InsertItem($"{i:D4}", null, cancellationToken);
				}

				// act
				var command = new ExportClassifierList
				{
					Request = new ClassifierSearchRequest
					{
						UserUid = Guid.NewGuid(),
						TypeCode = generator.TypeCode
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
					await result.Stream.CopyToAsync(fs, cancellationToken);
				}
			}
		}
	}
}
