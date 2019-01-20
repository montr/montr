using System;

namespace Montr.Core.Models
{
	public class AuditEvent
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public Guid CompanyUid { get; set; }

		public Guid UserUid { get; set; }

		public DateTime CreatedAtUtc { get; set; }

		public string MessageCode { get; set; }

		public object MessageParameters { get; set; }

		public string Ip { get; set; }

		public string Browser { get; set; }
	}
}
