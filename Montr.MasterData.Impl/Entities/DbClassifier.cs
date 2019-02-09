using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.MasterData.Impl.Entities
{
	[Table(Name = "classifier_type")]
	public class DbClassifierType
	{
		[Column(Name = "uid"), DataType(DataType.Guid), PrimaryKey, Identity]
		public System.Guid Uid { get; set; }
		
		[Column(Name = "company_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid CompanyUid { get; set; }

		[Column(Name = "config_code"), DataType(DataType.VarChar), NotNull]
		public string ConfigCode { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), Nullable]
		public string Name { get; set; }

		[Column(Name = "is_system"), DataType(DataType.Boolean), NotNull]
		public bool IsSystem { get; set; }

		[Column(Name = "table_name"), DataType(DataType.VarChar), Nullable]
		public string TableName { get; set; }
	}

	[Table(Name = "classifier")]
	public class DbClassifier
	{
		[Column(Name = "uid"), DataType(DataType.Guid), PrimaryKey, Identity]
		public System.Guid Uid { get; set; }

		[Column(Name = "company_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid CompanyUid { get; set; }

		[Column(Name = "config_code"), DataType(DataType.VarChar), NotNull]
		public string ConfigCode { get; set; }

		[Column(Name = "status_code"), DataType(DataType.VarChar), NotNull]
		public string StatusCode { get; set; }

		[Column(Name = "code"), DataType(DataType.VarChar), NotNull]
		public string Code { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), Nullable]
		public string Name { get; set; }
	}

	[Table(Name = "classifier_group")]
	public class DbClassifierGroup
	{
		[Column(Name = "uid"), DataType(DataType.Guid), PrimaryKey, Identity]
		public System.Guid Uid { get; set; }

		[Column(Name = "company_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid CompanyUid { get; set; }

		[Column(Name = "parent_uid"), DataType(DataType.Guid), Nullable]
		public System.Guid ParentUid { get; set; }

		[Column(Name = "code"), DataType(DataType.VarChar), NotNull]
		public string Code { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), Nullable]
		public string Name { get; set; }
	}

	[Table(Name = "classifier_group_link")]
	public class DbClassifierGroupLink
	{
		[Column(Name = "classifier_group_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid ClassifierGroupUid { get; set; }

		[Column(Name = "classifier_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid ClassifierUid { get; set; }
	}

	[Table(Name = "classifier_tree")]
	public class DbClassifierTree
	{
	}

	[Table(Name = "classifier_hierarchy")]
	public class DbClassifierHierarchy
	{
	}
}
