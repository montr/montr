using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Core.Entities
{
	[Table(Schema = "montr", Name = "migration")]
	public class DbMigration
	{
		[Column(Name = "id"), DataType(DataType.Int64), NotNull, PrimaryKey, Identity]
		public long Id { get; set; }

		[Column(Name = "file_name", Length = 500), DataType(DataType.VarChar), NotNull]
		public string FileName { get; set; }

		[Column(Name = "hash", Length = 40), DataType(DataType.VarChar), NotNull]
		public string Hash { get; set; }

		[Column(Name = "executed_at_utc"), DataType(DataType.DateTime2)]
		public DateTime ExecutedAtUtc { get; set; }

		[Column(Name = "duration_ms"), DataType(DataType.Int32)]
		public long DurationMs { get; set; }
	}
}
