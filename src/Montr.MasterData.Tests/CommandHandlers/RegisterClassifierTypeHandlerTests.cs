using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;

namespace Montr.MasterData.Tests.CommandHandlers
{
	[TestClass]
	public class RegisterClassifierTypeHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_RegisterClassifierType()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var dbFieldMetadataService = new DbFieldMetadataService(dbContextFactory, new NewtonsoftJsonSerializer());
			var handler = new RegisterClassifierTypeHandler(unitOfWorkFactory, classifierTypeService, dbFieldMetadataService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act
				var command = new RegisterClassifierType
				{
					Item = new ClassifierType
					{
						Code = "new_classifier_registration",
						Name = "New Classifier Registration",
						IsSystem = true
					},
					Fields = new List<FieldMetadata>
					{
						new TextField { Key = "code", Name = "Code", Active = true, System = true },
						new TextField { Key = "name", Name = "Name", Active = true, System = true },
					}
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreNotEqual(Guid.Empty, result.Uid);
			}
		}
	}
}
