using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Automate.Services.Implementations;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Services.CommandHandlers;
using Montr.MasterData.Services.Implementations;
using Montr.MasterData.Tests.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services.Implementations;
using Moq;
using NUnit.Framework;

namespace Montr.Automate.Tests.CommandHandlers
{
	public class AutomateClassifierHandlerTests
	{
		private static readonly string AutomationTypeCode = "automation_for_test";

		private static INamedServiceFactory<IClassifierRepository> CreateClassifierRepositoryFactory(IDbContextFactory dbContextFactory)
		{
			var jsonSerializer = new NewtonsoftJsonSerializer();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

			var fieldProviderRegistry = new DefaultFieldProviderRegistry();
			fieldProviderRegistry.AddFieldType(typeof(NumberField));
			fieldProviderRegistry.AddFieldType(typeof(TextField));
			fieldProviderRegistry.AddFieldType(typeof(TextAreaField));
			fieldProviderRegistry.AddFieldType(typeof(SelectField));
			fieldProviderRegistry.AddFieldType(typeof(AutomationConditionListField));
			fieldProviderRegistry.AddFieldType(typeof(AutomationActionListField));
			var dbFieldDataRepository = new DbFieldDataRepository(NullLogger<DbFieldDataRepository>.Instance, dbContextFactory, fieldProviderRegistry);

			var metadataServiceMock = new Mock<IClassifierTypeMetadataService>();
			metadataServiceMock
				.Setup(x => x.GetMetadata(It.IsAny<ClassifierType>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(() => Automation.GetDefaultMetadata().Fields.ToArray());

			var acpfMock = new Mock<INamedServiceFactory<IAutomationConditionProvider>>();
			acpfMock.Setup(x => x.GetRequiredService(FieldAutomationCondition.TypeCode))
				.Returns(new NoopAutomationConditionProvider { RuleType = new AutomationRuleType { Type = typeof(FieldAutomationCondition) } });

			var aapfMock = new Mock<INamedServiceFactory<IAutomationActionProvider>>();
			aapfMock.Setup(x => x.GetRequiredService(NotifyByEmailAutomationAction.TypeCode))
				.Returns(new NoopAutomationActionProvider { RuleType = new AutomationRuleType { Type =  typeof(NotifyByEmailAutomationAction) } });

			var automationRepository = new DbAutomationRepository(dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null,
				jsonSerializer, acpfMock.Object, aapfMock.Object);

			var classifierRepositoryFactoryMock = new Mock<INamedServiceFactory<IClassifierRepository>>();

			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == AutomationTypeCode)))
				.Returns(() => automationRepository);

			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name != AutomationTypeCode)))
				.Throws<InvalidOperationException>();

			return classifierRepositoryFactoryMock.Object;
		}

		[Test]
		public async Task ManageAutomation_NormalValues_ManageItems()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var classifierRepositoryFactory = CreateClassifierRepositoryFactory(dbContextFactory);
			var insertHandler = new InsertClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);
			var updateHandler = new UpdateClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);
			var deleteHandler = new DeleteClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				generator.TypeCode = AutomationTypeCode;

				await generator.InsertType(HierarchyType.None, cancellationToken);

				// act - insert
				var insertedIds = new List<Guid>();
				for (var i = 0; i < 5; i++)
				{
					var insertResult = await insertHandler.Handle(new InsertClassifier
					{
						UserUid = generator.UserUid,
						Item = new Automation
						{
							Type = generator.TypeCode,
							Code = "00" + i,
							Name = "00" + i + " - Test Automation",
							EntityTypeCode = "DocumentType",
							Conditions = new List<AutomationCondition>
							{
								new FieldAutomationCondition
								{
									Props = new FieldAutomationCondition.Properties
									{
										Field = "Status",
										Operator = AutomationConditionOperator.Equal,
										Value = "Published"
									}
								}
							},
							Actions = new List<AutomationAction>
							{
								new NotifyByEmailAutomationAction
								{
									Props = new NotifyByEmailAutomationAction.Properties
									{
										Recipient = "operator",
										Subject = "Test message #1",
										Body = "Hello"
									}
								}
							}
						}
					}, cancellationToken);

					Assert.That(insertResult, Is.Not.Null);
					Assert.That(insertResult.Success, Is.EqualTo(true));

					// ReSharper disable once PossibleInvalidOperationException
					insertedIds.Add(insertResult.Uid.Value);
				}

				var searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest {TypeCode = generator.TypeCode}, cancellationToken);

				// assert
				Assert.That(searchResult, Is.Not.Null);
				Assert.That(searchResult.Rows, Has.Count.EqualTo(insertedIds.Count));

				// act - update
				foreach (var classifier in searchResult.Rows.Cast<Automation>())
				{
					// todo: update automation specific properties
					classifier.Name = classifier.Name.Replace("Test", "Updated");

					var updateCommand = new UpdateClassifier
					{
						UserUid = generator.UserUid,
						Item = classifier
					};

					var updateResult = await updateHandler.Handle(updateCommand, cancellationToken);

					Assert.That(updateResult, Is.Not.Null);
					Assert.That(updateResult.Success, Is.EqualTo(true));
				}

				searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest {TypeCode = generator.TypeCode}, cancellationToken);

				// assert
				Assert.That(searchResult, Is.Not.Null);
				Assert.That(searchResult.Rows, Has.Count.EqualTo(insertedIds.Count));
				Assert.That(searchResult.Rows.Count(x => x.Name.Contains("Test")), Is.EqualTo(0));
				Assert.That(searchResult.Rows.Count(x => x.Name.Contains("Updated")), Is.EqualTo(insertedIds.Count));
				// Assert.AreEqual(insertedIds.Count, searchResult.Rows.Cast<Automation>().Count(x => x.Pattern.Contains("No.")));

				// act - delete
				var command = new DeleteClassifier
				{
					UserUid = generator.UserUid,
					TypeCode = generator.TypeCode,
					Uids = insertedIds.ToArray()
				};

				var result = await deleteHandler.Handle(command, cancellationToken);

				// assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Success);
				Assert.That(result.AffectedRows, Is.EqualTo(insertedIds.Count));

				searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest {TypeCode = generator.TypeCode}, cancellationToken);

				// assert
				Assert.That(searchResult, Is.Not.Null);
				Assert.That(searchResult.Rows, Is.Empty);
			}
		}

		private class NoopAutomationActionProvider : IAutomationActionProvider
		{
			public AutomationRuleType RuleType { get; init; }

			public Task<IList<FieldMetadata>> GetMetadata(
				AutomationContext context, AutomationAction action, CancellationToken cancellationToken = default)
			{
				return null;
			}

			public Task<ApiResult> Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken)
			{
				var result = new ApiResult();

				return Task.FromResult(result);
			}
		}

		private class NoopAutomationConditionProvider : IAutomationConditionProvider
		{
			public AutomationRuleType RuleType { get; init; }

			public Task<IList<FieldMetadata>> GetMetadata(
				AutomationContext context, AutomationCondition condition, CancellationToken cancellationToken)
			{
				return null;
			}

			public Task<bool> Meet(AutomationCondition automationCondition, AutomationContext context, CancellationToken cancellationToken)
			{
				return Task.FromResult(false);
			}
		}
	}
}
