using System.Collections.Generic;
using MediatR;
using Montr.Automate.Models;

namespace Montr.Automate.Queries
{
	public class GetAutomationActionTypeList : IRequest<IList<AutomationRuleType>>
	{
	}
}
