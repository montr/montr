using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;

namespace Montr.Automate.Services
{
	public interface IAutomationActionProvider
	{
		AutomationRuleType RuleType { get; }

		Task Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken);
	}
}
