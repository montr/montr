using System;
using System.Diagnostics;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Tasks.Impl.Entities
{
	[Table(Schema = "montr", Name = "task")]
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class DbTask
	{
		private string DebuggerDisplay => $"{Number}, {Name}";

		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey]
		public Guid Uid { get; set; }

		[Column(Name = "company_uid"), DataType(DataType.Guid), NotNull]
		public Guid CompanyUid { get; set; }

		[Column(Name = "task_type_uid"), DataType(DataType.Guid), NotNull]
		public Guid TaskTypeUid { get; set; }

		[Column(Name = "status_code", Length = 16), DataType(DataType.VarChar), NotNull]
		public string StatusCode { get; set; }

		[Column(Name = "number"), DataType(DataType.VarChar), Nullable]
		public string Number { get; set; }

		[Column(Name = "assignee_uid"), DataType(DataType.Guid), Nullable]
		public Guid? AssigneeUid { get; set; }

		[Column(Name = "parent_uid"), DataType(DataType.Guid), Nullable]
		public Guid? ParentUid { get; set; }

		[Column(Name = "name", Length = 2048), DataType(DataType.VarChar), Nullable]
		public string Name { get; set; }

		[Column(Name = "description"), DataType(DataType.VarChar), Nullable]
		public string Description { get; set; }

		[Column("start_date_utc"), DataType(DataType.DateTime2)]
		public DateTime? StartDateUtc { get; set; }

		[Column("due_date_utc"), DataType(DataType.DateTime2)]
		public DateTime? DueDateUtc { get; set; }

		[Column("created_at_utc"), DataType(DataType.DateTime2)]
		public DateTime? CreatedAtUtc { get; set; }

		[Column("created_by")]
		public Guid? CreatedBy { get; set; }

		[Column("modified_at_utc"), DataType(DataType.DateTime2)]
		public DateTime? ModifiedAtUtc { get; set; }

		[Column("modified_by")]
		public Guid? ModifiedBy { get; set; }
	}
}
