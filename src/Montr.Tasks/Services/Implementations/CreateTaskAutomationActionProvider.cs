using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Models;
using Montr.Tasks.Models;

namespace Montr.Tasks.Services.Implementations
{
	public class CreateTaskAutomationActionProvider : IAutomationActionProvider
	{
		private readonly IOptionsMonitor<AppOptions> _appOptionsMonitor;
		private readonly ITaskService _taskService;
		private readonly IEntityRelationService _entityRelationService;

		public CreateTaskAutomationActionProvider(IOptionsMonitor<AppOptions> appOptionsMonitor,
			ITaskService taskService, IEntityRelationService entityRelationService)
		{
			_appOptionsMonitor = appOptionsMonitor;
			_taskService = taskService;
			_entityRelationService = entityRelationService;
		}

		public AutomationRuleType RuleType => new()
		{
			Code = CreateTaskAutomationAction.TypeCode,
			Name = "Create Task",
			Type = typeof(CreateTaskAutomationAction)
		};

		public async Task<IList<FieldMetadata>> GetMetadata(
			AutomationContext context, AutomationAction action, CancellationToken cancellationToken = default)
		{
			return await Task.FromResult(new List<FieldMetadata>
			{
				new ClassifierField { Key = "taskTypeUid", Name = "Task type", Required = true, Props = { TypeCode = Models.ClassifierTypeCode.TaskType } },
				new ClassifierField { Key = "assigneeUid", Name = "Assignee", Required = true, Props = { TypeCode = Idx.ClassifierTypeCode.User } },
				new TextField { Key = "name", Name = "Name", Placeholder = "Name", Required = true },
				new TextAreaField { Key = "description", Name = "Description", Placeholder = "Description", Props = new TextAreaField.Properties { Rows = 2 } }
			});
		}

		public async Task Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken)
		{
			var action = (CreateTaskAutomationAction)automationAction;

			var appOptions = _appOptionsMonitor.CurrentValue;

			var model = new TaskModel
			{
				CompanyUid = appOptions.OperatorCompanyId,
				TaskTypeUid = action.Props.TaskTypeUid,
				AssigneeUid = action.Props.AssigneeUid,
				Name = action.Props.Name,
				Description = action.Props.Description
			};

			var result = await _taskService.Insert(model, cancellationToken);

			result.AssertSuccess(() => $"Failed to create task {model}");

			if (result.Uid.HasValue)
			{
				var relation = new EntityRelation
				{
					EntityTypeCode = EntityTypeCode.Task,
					EntityUid = result.Uid.Value,
					RelatedEntityTypeCode = context.EntityTypeCode,
					RelatedEntityUid = context.EntityUid,
					RelationType = RelationTypeCode.Context
				};

				await _entityRelationService.Insert(relation, cancellationToken);
			}
		}
	}
}
