using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Services;

namespace Montr.Automate.Impl.Services
{
	public class DefaultAutomationRunner : IAutomationRunner
	{
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;
		private readonly IAutomationProviderRegistry _providerRegistry;

		public DefaultAutomationRunner(
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory,
			IAutomationProviderRegistry providerRegistry)
		{
			_classifierRepositoryFactory = classifierRepositoryFactory;
			_providerRegistry = providerRegistry;
		}

		public async Task<ApiResult> Run(AutomationContext context, CancellationToken cancellationToken)
		{
			var classifierRepository = _classifierRepositoryFactory.GetService(ClassifierTypeCode.Automation);

			var automations = await classifierRepository.Search(new AutomationSearchRequest
			{
				EntityTypeCode = context.MetadataEntityTypeCode,
				// EntityUid = context.MetadataEntityUid,
				IsActive = true,
				IncludeRules = true,
				PageSize = 100
			}, cancellationToken);

			var affected = 0;

			foreach (var automation in automations.Rows.Cast<Automation>())
			{
				if (await MeetAll(automation.Conditions, context, cancellationToken))
				{
					await Execute(automation.Actions, context, cancellationToken);

					affected++;
				}
			}

			return new ApiResult { AffectedRows = affected };
		}

		private async Task<bool> MeetAll(IEnumerable<AutomationCondition> conditions, AutomationContext context, CancellationToken cancellationToken)
		{
			if (conditions != null)
			{
				foreach (var condition in conditions)
				{
					var provider = _providerRegistry.GetConditionProvider(condition.Type);

					if (await provider.Meet(condition, context, cancellationToken) == false)
					{
						return false;
					}
				}
			}

			return true;
		}

		private async Task Execute(IEnumerable<AutomationAction> actions, AutomationContext context, CancellationToken cancellationToken)
		{
			if (actions != null)
			{
				foreach (var action in actions)
				{
					var provider = _providerRegistry.GetActionProvider(action.Type);

					await provider.Execute(action, context, cancellationToken);
				}
			}
		}
	}
}
