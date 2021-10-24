using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core;
using Montr.Tasks.Models;
using Montr.Tasks.Services;

namespace Montr.Tasks.Impl.Services
{
	public class CreateTaskAutomationActionProvider : IAutomationActionProvider
	{
		private readonly IOptionsMonitor<AppOptions> _appOptionsMonitor;
		private readonly ITaskService _taskService;

		public CreateTaskAutomationActionProvider(IOptionsMonitor<AppOptions> appOptionsMonitor, ITaskService taskService)
		{
			_appOptionsMonitor = appOptionsMonitor;
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

			var appOptions = _appOptionsMonitor.CurrentValue;

			var model = new TaskModel
			{
				CompanyUid = appOptions.OperatorCompanyId,
				Name = action.Props.Name,
				Description = action.Props.Description
			};

			var result = await _taskService.Insert(model, cancellationToken);

			result.AssertSuccess(() => $"Failed to create task {model}");
		}
	}
}
