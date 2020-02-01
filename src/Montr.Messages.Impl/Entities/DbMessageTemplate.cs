using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Messages.Impl.Entities
{
	[Table(Schema = "montr", Name = "message_template")]
	public class DbMessageTemplate
	{
		[Column(Name = "uid"), DataType(DataType.Guid), PrimaryKey, Identity]
		public System.Guid Uid { get; set; }
		
		[Column(Name = "subject"), DataType(DataType.VarChar), Nullable]
		public string Subject { get; set; }

		[Column(Name = "body"), DataType(DataType.VarChar), Nullable]
		public string Body { get; set; }
	}
}
