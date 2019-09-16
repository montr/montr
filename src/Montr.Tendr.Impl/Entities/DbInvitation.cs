using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Tendr.Impl.Entities
{
	[Table(Name = "invitation")]
	public class DbInvitation
	{
		[Column(Name = "uid"), DataType(DataType.Guid), PrimaryKey, Identity]
		public System.Guid Uid { get; set; }

		[Column(Name = "event_uid"), DataType(DataType.Guid)]
		public System.Guid EventUid { get; set; }

		[Column(Name = "status_code"), DataType(DataType.VarChar), NotNull]
		public string StatusCode { get; set; }

		[Column(Name = "counterparty_uid"), DataType(DataType.Guid)]
		public System.Guid CounterpartyUid { get; set; }

		[Column(Name = "email"), DataType(DataType.VarChar)]
		public string Email { get; set; }

		// todo: generated code, invitation message...
	}
}
