using MediatR;
using Montr.Automate.Models;

namespace Montr.Automate.Queries
{
	public class GetAutomation : AutomationSearchRequest, IRequest<Automation>
	{
	}
}
