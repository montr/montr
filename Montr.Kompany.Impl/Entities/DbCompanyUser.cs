using LinqToDB;
using LinqToDB.Mapping;

namespace Kompany.Implementation.Entities
{
	[Table(Name = "company_user")]
	public class DbCompanyUser
	{
		[Column(Name = "company_uid"), DataType(DataType.Guid), PrimaryKey(0), NotNull]
		public System.Guid CompanyUid { get; set; }

		[Column(Name = "user_uid"), DataType(DataType.Guid), PrimaryKey(1), NotNull]
		public System.Guid UserUid { get; set; }

	}
}
