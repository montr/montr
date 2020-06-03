using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Automate.Impl.Entities
{
	[Table(Schema = "montr", Name = "automation")]
	public class DbAutomation
	{
		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey, Identity]
		public Guid Uid { get; set; }

		[Column(Name = "entity_type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string EntityTypeCode { get; set; }

		[Column(Name = "entity_type_uid"), DataType(DataType.Guid), NotNull]
		public Guid EntityTypeUid { get; set; }

		/// <summary>
		/// trigger | monitor | schedule (date-based)
		/// </summary>
		[Column(Name = "type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string TypeCode { get; set; }

		[Column(Name = "display_order"), DataType(DataType.Int32), NotNull]
		public int DisplayOrder { get; set; }

		[Column(Name = "name", Length = 128), DataType(DataType.VarChar)]
		public string Name { get; set; }

		[Column(Name = "description"), DataType(DataType.VarChar)]
		public string Description { get; set; }

		[Column(Name = "is_active"), DataType(DataType.Boolean), NotNull]
		public bool IsActive { get; set; }

		[Column(Name = "is_system"), DataType(DataType.Boolean), NotNull]
		public bool IsSystem { get; set; }

		[Column(Name = "created_by"), DataType(DataType.Guid)]
		public Guid CreatedBy { get; set; }

		[Column(Name = "created_at_utc"), DataType(DataType.DateTime2)]
		public DateTime CreatedAtUtc { get; set; }

		[Column(Name = "modified_by"), DataType(DataType.Guid)]
		public Guid ModifiedBy { get; set; }

		[Column(Name = "modified_at_utc"), DataType(DataType.DateTime2)]
		public DateTime ModifiedAtUtc { get; set; }
	}

	[Table(Schema = "montr", Name = "automation_condition")]
	public class DbAutomationCondition
	{
		[Column(Name = "automation_uid"), DataType(DataType.Guid), NotNull]
		public Guid AutomationUid { get; set; }

		/// <summary>
		/// all | any
		/// </summary>
		public string Meet { get; set; }

		[Column(Name = "position"), DataType(DataType.Int32), NotNull]
		public int Position { get; set; }

		public string Field { get; set; }

		public string Operator { get; set; }

		public string Value { get; set; }
	}

	[Table(Schema = "montr", Name = "automation_action")]
	public class DbAutomationAction
	{
		[Column(Name = "automation_uid"), DataType(DataType.Guid), NotNull]
		public Guid AutomationUid { get; set; }

		[Column(Name = "position"), DataType(DataType.Int32), NotNull]
		public int Position { get; set; }

		public string Field { get; set; }

		public string Value { get; set; }
	}
}
