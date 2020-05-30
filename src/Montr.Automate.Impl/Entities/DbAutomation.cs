using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Metadata.Impl.Entities
{
	[Table(Schema = "montr", Name = "automation")]
	public class DbAutomation
	{
		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey, Identity]
		public System.Guid Uid { get; set; }

		[Column(Name = "entity_type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string EntityTypeCode { get; set; }

		[Column(Name = "entity_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid EntityUid { get; set; }

		/// <summary>
		/// trigger | schedule (date-based)
		/// </summary>
		[Column(Name = "type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string Type { get; set; }

		[Column(Name = "is_active"), DataType(DataType.Boolean), NotNull]
		public bool IsActive { get; set; }

		[Column(Name = "name", Length = 128), DataType(DataType.VarChar)]
		public string Name { get; set; }

		[Column(Name = "description"), DataType(DataType.VarChar)]
		public string Description { get; set; }

		[Column(Name = "created_by"), DataType(DataType.Guid)]
		public System.Guid CreatedBy { get; set; }

		[Column(Name = "created_at_utc"), DataType(DataType.DateTime2)]
		public System.DateTime CreatedAtUtc { get; set; }

		[Column(Name = "modified_by"), DataType(DataType.Guid)]
		public System.Guid ModifiedBy { get; set; }

		[Column(Name = "modified_at_utc"), DataType(DataType.DateTime2)]
		public System.DateTime ModifiedAtUtc { get; set; }
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
