using System;

namespace Montr.Docs.Models
{
	public class Document
	{
		public Guid Uid { get; set; }

		public Guid CompanyUid { get; set; }

		public string ConfigCode { get; set; }

		public string StatusCode { get; set; }

		public DocumentDirection Direction { get; set; }

		public string DocumentNumber { get; set; }

		public DateTime DocumentDate { get; set; }

		public string Name { get; set; }

		public DateTime? CreatedAtUtc { get; set; }

		public string CreatedBy { get; set; }

		public DateTime? ModifiedAtUtc { get; set; }

		public string ModifiedBy { get; set; }

		public string Url { get; set; }
	}

	public enum DocumentDirection
	{
		Internal,

		Incoming,

		Outgoing
	}
}
