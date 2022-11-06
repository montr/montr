using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services.Implementations;
using Montr.Metadata.Models;
using Montr.Metadata.Services.Implementations;
using NUnit.Framework;

namespace Montr.MasterData.Tests.Services
{
	[TestFixture]
	public class DefaultClassifierTypeRegistratorTests
	{
		[Test]
		public async Task Register_NormalValues_RegisterClassifierType()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var dbFieldMetadataService = new DbFieldMetadataService(dbContextFactory, new DefaultDateTimeProvider(), new NewtonsoftJsonSerializer());

			var handler = new DefaultClassifierTypeRegistrator(
				new NullLogger<DefaultClassifierTypeRegistrator>(),
				unitOfWorkFactory, classifierTypeService, dbFieldMetadataService);

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

				var result = await handler.Register(command.Item, command.Fields, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreNotEqual(Guid.Empty, result.Uid);
			}
		}
	}
}
