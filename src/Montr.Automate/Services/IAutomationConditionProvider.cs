using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Metadata.Models;

namespace Montr.Automate.Services
{
	public interface IAutomationConditionProvider
	{
		AutomationRuleType RuleType { get; }

		// todo: merge with RuleType property (?)
		IList<FieldMetadata> GetMetadata();

		Task<bool> Meet(AutomationCondition automationCondition, AutomationContext context, CancellationToken cancellationToken);
	}
}
