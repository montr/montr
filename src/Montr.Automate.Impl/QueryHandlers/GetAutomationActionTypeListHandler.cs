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
	public class GetAutomationActionTypeListHandler : IRequestHandler<GetAutomationActionTypeList, IList<AutomationRuleType>>
	{
		private readonly INamedServiceFactory<IAutomationActionProvider> _providerFactory;

		public GetAutomationActionTypeListHandler(
			INamedServiceFactory<IAutomationActionProvider> providerFactory)
		{
			_providerFactory = providerFactory;
		}

		public async Task<IList<AutomationRuleType>> Handle(GetAutomationActionTypeList request,
			CancellationToken cancellationToken)
		{
			var result = _providerFactory.GetNames()
				.Select(x => _providerFactory.GetRequiredService(x).RuleType).ToList();

			return await Task.FromResult(result);
		}
	}
}
