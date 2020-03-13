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
		private string DebuggerDisplay => $"{Name} - {Pattern}";

		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey]
		public Guid Uid { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), NotNull]
		public string Name { get; set; }

		[Column(Name = "pattern"), DataType(DataType.VarChar), NotNull]
		public string Pattern { get; set; }

		[Column(Name = "periodicity"), DataType(DataType.VarChar), NotNull]
		public string Periodicity { get; set; }

		[Column(Name = "is_active"), DataType(DataType.Boolean), NotNull]
		public bool IsActive { get; set; }

		[Column(Name = "is_system"), DataType(DataType.Boolean), NotNull]
		public bool IsSystem { get; set; }
	}

	[Table(Schema = "montr", Name = "numerator_entity")]
	public class DbNumeratorEntity
	{
	}

	[Table(Schema = "montr", Name = "numerator_counter")]
	public class DbNumeratorCounter
	{
		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey]
		public Guid Uid { get; set; }

		[Column(Name = "numerator_uid"), DataType(DataType.Guid), NotNull]
		public Guid NumeratorUid { get; set; }

		[Column(Name = "key"), DataType(DataType.VarChar), NotNull]
		public string Key { get; set; }

		[Column(Name = "value"), DataType(DataType.Long), NotNull]
		public long Value { get; set; }
	}

	[Table(Schema = "montr", Name = "numerator_counter_param")]
	public class DbNumeratorCounterParam
	{
		public string Key { get; set; }

		public string Value { get; set; }
	}
}
