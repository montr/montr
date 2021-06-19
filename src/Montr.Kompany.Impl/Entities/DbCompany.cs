using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Kompany.Impl.Entities
{
	[Table(Schema = "montr", Name = "company")]
	public class DbCompany
	{
		[Column(Name = "uid"), DataType(DataType.Guid), PrimaryKey, Identity]
		public Guid Uid { get; set; }

		[Column(Name = "config_code"), DataType(DataType.VarChar), NotNull]
		public string ConfigCode { get; set; }
	}
}
