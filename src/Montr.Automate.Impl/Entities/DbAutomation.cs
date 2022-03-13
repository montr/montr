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

		/// <summary>
		/// classifier | document | task ... etc.
		/// </summary>
		[Column(Name = "entity_type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string EntityTypeCode { get; set; }

		/// <summary>
		/// trigger | monitor | schedule (date-based)
		/// </summary>
		[Column(Name = "type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string TypeCode { get; set; }
	}
}
