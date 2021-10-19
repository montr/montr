using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Tasks.Models;
using Montr.Tasks.Services;

namespace Montr.Tasks.Impl.Services
{
	public class CreateTaskAutomationActionProvider : IAutomationActionProvider
	{
		private readonly ITaskService _taskService;

		public CreateTaskAutomationActionProvider(ITaskService taskService)
		{
			_taskService = taskService;
		}

		public AutomationRuleType RuleType => new()
		{
			Code = CreateTaskAutomationAction.TypeCode,
			Name = "Create Task",
			Type = typeof(CreateTaskAutomationAction)
		};

		public async Task Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken)
		{
			var action = (CreateTaskAutomationAction)automationAction;

			var model = new TaskModel
			{
				Name = action.Props.Name,
				Description = action.Props.Description
			};

			var result = await _taskService.Insert(model, cancellationToken);

			result.AssertSuccess(() => $"Failed to create task {model}");
		}
	}
}
