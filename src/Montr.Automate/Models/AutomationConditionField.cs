using Montr.Automate.Services;
using Montr.Metadata.Models;

namespace Montr.Automate.Models
{
	[FieldType(Code, typeof(AutomationConditionFieldProvider))]
	public class AutomationConditionField : FieldMetadata
	{
		public const string Code = "automation-condition";

		public override string Type => Code;
	}

	[FieldType(Code, typeof(AutomationActionListFieldProvider))]
	public class AutomationActionListField : FieldMetadata
	{
		public const string Code = "automation-action-list";

		public override string Type => Code;
	}
}
