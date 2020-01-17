using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Docs.Impl.Entities
{
	[Table(Name = "document")]
	public class DbDocument
	{
		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey, Identity]
		public System.Guid Uid { get; set; }

		[Column(Name = "company_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid CompanyUid { get; set; }

		[Column(Name = "config_code"), DataType(DataType.VarChar), NotNull]
		public string ConfigCode { get; set; }

		[Column(Name = "status_code"), DataType(DataType.VarChar), NotNull]
		public string StatusCode { get; set; }
	}
}
