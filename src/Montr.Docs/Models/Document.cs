using System;
using System.Diagnostics;
using Montr.Metadata.Models;

namespace Montr.Docs.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Document : IFieldDataContainer
	{
		private string DebuggerDisplay => $"{StatusCode}, {Name}";

		public static readonly string TypeCode = nameof(Document);

		public Guid? Uid { get; set; }

		public Guid CompanyUid { get; set; }

		public Guid DocumentTypeUid { get; set; }

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

		public FieldData Fields { get; set; }
	}

	public enum DocumentDirection
	{
		Internal,

		Incoming,

		Outgoing
	}
}
