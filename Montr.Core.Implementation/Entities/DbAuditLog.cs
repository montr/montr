using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Core.Implementation.Entities
{
	[Table(Name = "audit_log")]
	public class DbAuditLog
	{
		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey, Identity]
		public System.Guid Uid { get; set; }

		[Column(Name = "entity_type_code"), DataType(DataType.VarChar), NotNull]
		public string EntityTypeCode { get; set; }

		[Column(Name = "entity_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid EntityUid { get; set; }

		[Column(Name = "company_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid CompanyUid { get; set; }

		[Column(Name = "user_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid UserUid { get; set; }

		[Column(Name = "created_at_utc"), DataType(DataType.DateTime2), NotNull]
		public System.DateTime CreatedAtUtc { get; set; }

		[Column(Name = "message_code"), DataType(DataType.VarChar), NotNull]
		public string MessageCode { get; set; }

		[Column(Name = "message_params"), DataType(DataType.NText)]
		public string MessageParameters { get; set; }

		[Column(Name = "ip"), DataType(DataType.VarChar)]
		public string Ip { get; set; }

		[Column(Name = "browser"), DataType(DataType.NText)]
		public string Browser { get; set; }
	}
}
