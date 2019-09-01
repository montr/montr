using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Tendr.Impl.Entities
{
	[Table(Name = "invitation")]
	public class DbInvitation
	{
		[Column(Name = "uid"), DataType(DataType.Guid), PrimaryKey, Identity]
		public System.Guid Uid { get; set; }

		[Column(Name = "company_uid"), DataType(DataType.Guid)]
		public System.Guid CompanyUid { get; set; }
	}
}