using Montr.Automate.Services;
using Montr.Metadata.Models;

namespace Montr.Automate.Models
{
	[FieldType(Code, typeof(AutomationConditionListProvider))]
	public class AutomationConditionList : FieldMetadata<AutomationConditionList.Properties>
	{
		public const string Code = "automation-condition-list";

		public override string Type => Code;

		public class Properties
		{
			public AutomationConditionMeet Meet { get; set; }
		}
	}
}
