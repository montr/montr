using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Tendr.Entities
{
	[Table(Schema = "montr", Name = "event")]
	public class DbEvent
	{
		[Column(Name = "uid"), DataType(DataType.Guid), PrimaryKey, Identity]
		public Guid Uid { get; set; }

		[Column(Name = "id"), DataType(DataType.Int64)]
		public long Id { get; set; }

		[Column(Name = "company_uid"), DataType(DataType.Guid)]
		public Guid CompanyUid { get; set; }

		[Column(Name = "is_template"), DataType(DataType.Boolean)]
		public bool IsTemplate { get; set; }

		[Column(Name = "template_uid"), DataType(DataType.Guid)]
		public Guid? TemplateUid { get; set; }

		[Column(Name = "config_code"), DataType(DataType.VarChar), NotNull]
		public string ConfigCode { get; set; }

		[Column(Name = "status_code"), DataType(DataType.VarChar), NotNull]
		public string StatusCode { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), Nullable]
		public string Name { get; set; }

		[Column(Name = "description"), DataType(DataType.VarChar), Nullable]
		public string Description { get; set; }
	}
}
