using Montr.Automate.Models;
using Montr.Metadata.Services.Implementations;

namespace Montr.Automate.Services.Implementations
{
	// todo: remove attribute, move to impl
	public class AutomationConditionListFieldProvider : DefaultFieldProvider<AutomationConditionListField, AutomationCondition[]>
	{
	}

	public class AutomationActionListFieldProvider : DefaultFieldProvider<AutomationActionListField, AutomationAction[]>
	{
	}
}
