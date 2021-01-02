using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Core.Impl.Entities
{
	[Table(Schema = "montr", Name = "settings")]
	public class DbSettings
	{
		[Column(Name = "id"), DataType(DataType.VarChar), NotNull, PrimaryKey]
		public string Id { get; set; }

		[Column(Name = "value"), DataType(DataType.VarChar)]
		public string Value { get; set; }
	}
}
