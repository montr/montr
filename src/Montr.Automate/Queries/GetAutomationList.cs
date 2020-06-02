using MediatR;
using Montr.Automate.Models;
using Montr.Core.Models;

namespace Montr.Automate.Queries
{
	public class GetAutomationList : AutomationSearchRequest, IRequest<SearchResult<Automation>>
	{
	}
}
