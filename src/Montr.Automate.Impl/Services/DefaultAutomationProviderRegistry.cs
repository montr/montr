using Montr.Automate.Services;
using Montr.Core.Services;

namespace Montr.Automate.Impl.Services
{
	public class DefaultAutomationProviderRegistry : IAutomationProviderRegistry
	{
		private readonly INamedServiceFactory<IAutomationConditionProvider> _conditionProviderFactory;
		private readonly INamedServiceFactory<IAutomationActionProvider> _actionProviderFactory;

		public DefaultAutomationProviderRegistry(
			INamedServiceFactory<IAutomationConditionProvider> conditionProviderFactory,
			INamedServiceFactory<IAutomationActionProvider> actionProviderFactory)
		{
			_conditionProviderFactory = conditionProviderFactory;
			_actionProviderFactory = actionProviderFactory;
		}

		public IAutomationConditionProvider GetConditionProvider(string typeCode)
		{
			return _conditionProviderFactory.Resolve(typeCode);
		}

		public IAutomationActionProvider GetActionProvider(string typeCode)
		{
			return _actionProviderFactory.Resolve(typeCode);
		}
	}
}
