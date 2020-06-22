using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Automate.Impl.Entities
{
	[Table(Schema = "montr", Name = "automation_condition")]
	public class DbAutomationCondition
	{
		[Column(Name = "automation_uid"), DataType(DataType.Guid), NotNull]
		public Guid AutomationUid { get; set; }

		/// <summary>
		/// all | any
		/// </summary>
		// public string Meet { get; set; }

		[Column(Name = "display_order"), DataType(DataType.Int32), NotNull]
		public int DisplayOrder { get; set; }

		[Column(Name = "condition_uid"), DataType(DataType.Guid)]
		public Guid? ConditionUid { get; set; }

		[Column(Name = "type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string TypeCode { get; set; }

		[Column(Name = "props"), DataType(DataType.NText)]
		public string Props { get; set; }
	}
}
