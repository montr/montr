using Montr.Automate.Services;
using Montr.Automate.Services.Implementations;
using Montr.Metadata.Models;

namespace Montr.Automate.Models
{
	[FieldType(Code, typeof(AutomationConditionListFieldProvider))]
	public class AutomationConditionListField : FieldMetadata
	{
		public const string Code = "automation-condition-list";

		public override string Type => Code;
	}

	[FieldType(Code, typeof(AutomationActionListFieldProvider))]
	public class AutomationActionListField : FieldMetadata
	{
		public const string Code = "automation-action-list";

		public override string Type => Code;
	}
}
