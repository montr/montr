using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Montr.Automate.Models;
using Montr.Core;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.Kompany.Models;
using Montr.Kompany.Tests.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Services.Implementations;
using Montr.MasterData.Tests.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services.Implementations;
using Montr.Tasks.Models;
using Montr.Tasks.Services.Implementations;
using Moq;
using NUnit.Framework;

namespace Montr.Tasks.Tests.Services
{
	[TestFixture]
	public class CreateTaskAutomationActionProviderTests
	{
		private static INamedServiceFactory<IClassifierRepository> CreateClassifierRepositoryFactory(IDbContextFactory dbContextFactory)
		{
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

			var fieldProviderRegistry = new DefaultFieldProviderRegistry();
			fieldProviderRegistry.AddFieldType(typeof(TextField));
			var dbFieldDataRepository = new DbFieldDataRepository(NullLogger<DbFieldDataRepository>.Instance, dbContextFactory, fieldProviderRegistry);

			var metadataServiceMock = new Mock<IClassifierTypeMetadataService>();
			metadataServiceMock
				.Setup(x => x.GetMetadata(It.IsAny<ClassifierType>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(() => new FieldMetadata[]
				{
					new TextField { Key = "test1", Active = true, System = false },
					new TextField { Key = "test2", Active = true, System = false },
					new TextField { Key = "test3", Active = true, System = false }
				});

			var classifierRepository = new DbClassifierRepository<Classifier>(dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null);

			var numeratorRepository = new DbNumeratorRepository(dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null);

			var taskTypeRepository = new DbTaskTypeRepository(dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null);

			var classifierRepositoryFactoryMock = new Mock<INamedServiceFactory<IClassifierRepository>>();

			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == ClassifierTypeCode.TaskType)))
				.Returns(() => taskTypeRepository);
			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == MasterData.ClassifierTypeCode.Numerator)))
				.Returns(() => numeratorRepository);
			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name =>
					name != ClassifierTypeCode.TaskType &&
					name != MasterData.ClassifierTypeCode.Numerator)))
				.Returns(() => classifierRepository);

			return classifierRepositoryFactoryMock.Object;
		}

		[Test]
		public async Task Execute_NormalCondition_ShouldCreateTask()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				var dbContextFactory = new DefaultDbContextFactory();

				var companyGenerator = new CompanyDbGenerator(unitOfWorkFactory, dbContextFactory);
				var operatorCompany = await companyGenerator.InsertCompany(new Company
				{
					Name = "Company 1",
					ConfigCode = CompanyConfigCode.Company
				}, cancellationToken);

				var masterDataGenerator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
				await masterDataGenerator.EnsureClassifierTypeRegistered(TaskType.GetDefaultMetadata(), cancellationToken);

				var tasksRepositoryFactory = CreateClassifierRepositoryFactory(dbContextFactory);

				var taskTypeRepository = tasksRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.TaskType);
				var taskType = await taskTypeRepository.Insert(new TaskType { Code = "001", Name = "Test task type" }, cancellationToken);

				var numerator = await masterDataGenerator.InsertNumerator(
					new Numerator { Pattern = "T-{Number}" },
					new GenerateNumberRequest { EntityTypeCode = "task" },
					cancellationToken);

				var appOptionsMock = new Mock<IOptionsMonitor<AppOptions>>();
				appOptionsMock.Setup(x => x.CurrentValue).Returns(() => new AppOptions
				{
					OperatorCompanyId = operatorCompany.Uid
				});

				var tasksOptionsMock = new Mock<IOptionsMonitor<TasksOptions>>();
				tasksOptionsMock.Setup(x => x.CurrentValue).Returns(() => new TasksOptions
				{
					DefaultNumeratorId = numerator.Uid
				});

				var dateTimeProvider = new DefaultDateTimeProvider();
				var numberTagResolvers = new INumberTagResolver[] { };
				var dbNumberGenerator = new DbNumberGenerator(dbContextFactory, tasksRepositoryFactory, dateTimeProvider, numberTagResolvers);
				var dbTaskService = new DbTaskService(dbContextFactory, dateTimeProvider, tasksOptionsMock.Object, dbNumberGenerator);
				var dbEntityRelationService = new DbEntityRelationService(dbContextFactory);
				var dbTaskRepository = new DbTaskRepository(dbContextFactory);
				var handler = new CreateTaskAutomationActionProvider(appOptionsMock.Object, dbTaskService, dbEntityRelationService);

				// act
				var action = new CreateTaskAutomationAction
				{
					Props = new CreateTaskAutomationAction.Properties
					{
						Name = "Test task",
						Description = "Test task description",
						TaskTypeUid = taskType.Uid
					}
				};
				var automationContext = new AutomationContext { EntityTypeCode = "document" };
				var result = await handler.Execute(action, automationContext, cancellationToken);

				// assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Success);
				Assert.That(result.AffectedRows, Is.EqualTo(1));
				Assert.That(result.Uid, Is.Not.Null);

				var tasks = await dbTaskRepository.Search(new TaskSearchRequest { Uid = result.Uid }, cancellationToken);
				Assert.That(tasks, Is.Not.Null);
				Assert.That(tasks.Rows, Has.Count.EqualTo(1));

				var task = tasks.Rows[0];
				Assert.That(result.Uid, Is.EqualTo(task.Uid));
				Assert.That(operatorCompany.Uid, Is.EqualTo(task.CompanyUid));
				Assert.That(action.Props.Name, Is.EqualTo(task.Name));
				Assert.That(action.Props.Description, Is.EqualTo(task.Description));
				Assert.That(taskType.Uid, Is.EqualTo(task.TaskTypeUid));
				Assert.That(task.Number, Is.Not.Null);
			}
		}
	}
}
