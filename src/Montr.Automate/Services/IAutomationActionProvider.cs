using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Metadata.Models;

namespace Montr.Automate.Services
{
	public interface IAutomationActionProvider
	{
		AutomationRuleType RuleType { get; }

		// todo: merge with RuleType property (?)
		Task<IList<FieldMetadata>> GetMetadata(
			AutomationContext context, AutomationAction action, CancellationToken cancellationToken = default);

		Task Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken = default);
	}
}
