using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Impl.Models;
using Montr.Automate.Models;
using Montr.Automate.Services;

namespace Montr.Automate.Impl.Services
{
	public class SetFieldAutomationActionProvider : IAutomationActionProvider
	{
		public AutomationRuleType RuleType => new AutomationRuleType
		{
			Code = SetFieldAutomationAction.TypeCode,
			Name = "Set Field",
			Type = typeof(SetFieldAutomationAction)
		};

		public Task Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}
	}
}
