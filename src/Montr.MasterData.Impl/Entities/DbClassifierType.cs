using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.MasterData.Impl.Entities
{
	[Table(Name = "classifier_type")]
	public class DbClassifierType
	{
		[Column(Name = "uid"), DataType(DataType.Guid), PrimaryKey, Identity]
		public Guid Uid { get; set; }

		[Column(Name = "company_uid"), DataType(DataType.Guid), NotNull]
		public Guid CompanyUid { get; set; }

		[Column(Name = "code"), DataType(DataType.VarChar), NotNull]
		public string Code { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), NotNull]
		public string Name { get; set; }

		[Column(Name = "description"), DataType(DataType.VarChar)]
		public string Description { get; set; }

		[Column(Name = "hierarchy_type"), DataType(DataType.VarChar), NotNull]
		public string HierarchyType { get; set; }

		/*[Column(Name = "is_system"), DataType(DataType.Boolean), NotNull]
		public bool IsSystem { get; set; }*/

		/*[Column(Name = "table_name"), DataType(DataType.VarChar), Nullable]
		public string TableName { get; set; }*/
	}
}
