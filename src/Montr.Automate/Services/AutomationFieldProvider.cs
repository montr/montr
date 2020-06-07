using Montr.Automate.Models;
using Montr.Metadata.Services;

namespace Montr.Automate.Services
{
	public class AutomationConditionFieldProvider : DefaultFieldProvider<AutomationConditionField, AutomationCondition>
	{
	}

	public class AutomationActionListFieldProvider : DefaultFieldProvider<AutomationActionListField, AutomationAction[]>
	{
	}
}
