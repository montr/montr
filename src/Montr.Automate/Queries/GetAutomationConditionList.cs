using System.Collections.Generic;
using MediatR;
using Montr.Automate.Models;

namespace Montr.Automate.Queries
{
	public class GetAutomationConditionList : IRequest<IList<AutomationCondition>>
	{
	}
}
