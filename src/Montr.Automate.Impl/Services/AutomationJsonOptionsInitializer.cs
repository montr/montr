using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Services;
using Montr.Core.Services.Impl;

namespace Montr.Automate.Impl.Services
{
	public class AutomationJsonOptionsInitializer : IStartupTask
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly JsonTypeProvider<AutomationCondition> _conditionTypeProvider;
		private readonly JsonTypeProvider<AutomationAction> _actionTypeProvider;

		public AutomationJsonOptionsInitializer(IServiceProvider serviceProvider,
			JsonTypeProvider<AutomationCondition> conditionTypeProvider, JsonTypeProvider<AutomationAction> actionTypeProvider)
		{
			_serviceProvider = serviceProvider;
			_conditionTypeProvider = conditionTypeProvider;
			_actionTypeProvider = actionTypeProvider;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var conditionProviderFactory = scope.ServiceProvider.GetRequiredService<INamedServiceFactory<IAutomationConditionProvider>>();
				foreach (var name in conditionProviderFactory.GetNames())
				{
					_conditionTypeProvider.Map[name] = conditionProviderFactory.GetRequiredService(name).RuleType.Type;
				}

				var actionProviderFactory = scope.ServiceProvider.GetRequiredService<INamedServiceFactory<IAutomationActionProvider>>();
				foreach (var name in actionProviderFactory.GetNames())
				{
					_actionTypeProvider.Map[name] = actionProviderFactory.GetRequiredService(name).RuleType.Type;
				}
			}

			return Task.CompletedTask;
		}
	}
}
