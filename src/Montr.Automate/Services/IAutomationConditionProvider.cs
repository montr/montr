using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;

namespace Montr.Automate.Services
{
	public interface IAutomationConditionProvider
	{
		AutomationRuleType RuleType { get; }

		Task<bool> Meet(AutomationCondition automationCondition, AutomationContext context, CancellationToken cancellationToken);
	}
}
