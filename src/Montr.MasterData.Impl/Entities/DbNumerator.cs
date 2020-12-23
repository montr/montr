using System;
using System.Diagnostics;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.MasterData.Impl.Entities
{
	[Table(Schema = "montr", Name = "numerator")]
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class DbNumerator
	{
		private string DebuggerDisplay => $"{EntityTypeCode} - {Pattern}";

		public static readonly string KeyTagsSeparator = ",";

		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey]
		public Guid Uid { get; set; }

		[Column(Name = "entity_type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string EntityTypeCode { get; set; }

		[Column(Name = "pattern"), DataType(DataType.VarChar), NotNull]
		public string Pattern { get; set; }

		[Column(Name = "periodicity"), DataType(DataType.VarChar), NotNull]
		public string Periodicity { get; set; }

		[Column(Name = "key_tags"), DataType(DataType.VarChar)]
		public string KeyTags { get; set; }
	}

	[Table(Schema = "montr", Name = "numerator_counter")]
	public class DbNumeratorCounter
	{
		[Column(Name = "numerator_uid"), DataType(DataType.Guid), NotNull, PrimaryKey(0)]
		public Guid NumeratorUid { get; set; }

		[Column(Name = "key"), DataType(DataType.VarChar), NotNull, PrimaryKey(1)]
		public string Key { get; set; }

		[Column(Name = "value"), DataType(DataType.Long), NotNull]
		public long Value { get; set; }

		[Column(Name = "generated_at_utc"), DataType(DataType.DateTime2)]
		public DateTime GeneratedAtUtc { get; set; }
	}

	[Table(Schema = "montr", Name = "numerator_entity")]
	public class DbNumeratorEntity
	{
		[Column(Name = "entity_uid"), DataType(DataType.Guid), NotNull, PrimaryKey]
		public Guid EntityUid { get; set; }

		[Column(Name = "is_autonumbering"), DataType(DataType.Boolean), NotNull]
		public bool IsAutoNumbering { get; set; }

		[Column(Name = "numerator_uid"), DataType(DataType.Guid)]
		public Guid NumeratorUid { get; set; }
	}
}
