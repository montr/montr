using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Kompany.Entities
{
	[Table(Schema = "montr", Name = "company_user")]
	public class DbCompanyUser
	{
		[Column(Name = "company_uid"), DataType(DataType.Guid), PrimaryKey(0), NotNull]
		public Guid CompanyUid { get; set; }

		[Column(Name = "user_uid"), DataType(DataType.Guid), PrimaryKey(1), NotNull]
		public Guid UserUid { get; set; }

	}
}
