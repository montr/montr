using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Automate.Entities
{
	[Table(Schema = "montr", Name = "automation")]
	public class DbAutomation
	{
		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey, Identity]
		public Guid Uid { get; set; }

		/// <summary>
		/// classifier | document | task ... etc.
		/// </summary>
		[Column(Name = "entity_type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string EntityTypeCode { get; set; }

		/// <summary>
		/// trigger | monitor | schedule (date-based)
		/// </summary>
		[Column(Name = "automation_type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string AutomationTypeCode { get; set; }
	}
}
