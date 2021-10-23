using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Automate.Models;
using Montr.Automate.Queries;
using Montr.Automate.Services;
using Montr.Core.Services;

namespace Montr.Automate.Impl.QueryHandlers
{
	public class GetAutomationConditionTypeListHandler : IRequestHandler<GetAutomationConditionTypeList, IList<AutomationRuleType>>
	{
		private readonly INamedServiceFactory<IAutomationConditionProvider> _providerFactory;

		public GetAutomationConditionTypeListHandler(
			INamedServiceFactory<IAutomationConditionProvider> providerFactory)
		{
			_providerFactory = providerFactory;
		}

		public async Task<IList<AutomationRuleType>> Handle(GetAutomationConditionTypeList request,
			CancellationToken cancellationToken)
		{
			var result = _providerFactory.GetNames().OrderBy(x => x)
				.Select(x => _providerFactory.GetRequiredService(x).RuleType).ToList();

			return await Task.FromResult(result);
		}
	}
}
