using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Idx.Impl.Entities
{
	[Table(Schema = "montr", Name = "team")]
	public class DbTeam
	{
		[Column(Name = "business_unit_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string BusinessUnitTypeCode { get; set; }

		[Column(Name = "business_unit_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid BusinessUnitUid { get; set; }

		[Column(Name = "role_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid RoleUid { get; set; }

		[Column(Name = "entity_type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string EntityTypeCode { get; set; }

		[Column(Name = "entity_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid EntityUid { get; set; }
	}
}
