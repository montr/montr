using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Core.Impl.Entities;

[Table(Schema = "montr", Name = "locale_string")]
public class DbLocaleString
{
	[Column(Name = "locale"), DataType(DataType.VarChar), NotNull, PrimaryKey(0)]
	public string Locale { get; set; }

	[Column(Name = "module"), DataType(DataType.VarChar), NotNull, PrimaryKey(1)]
	public string Module { get; set; }

	[Column(Name = "key"), DataType(DataType.VarChar), NotNull, PrimaryKey(2)]
	public string Key { get; set; }

	[Column(Name = "value"), DataType(DataType.Text), NotNull]
	public string Value { get; set; }
}
