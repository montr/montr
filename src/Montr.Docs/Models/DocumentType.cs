using System;
using System.Diagnostics;

namespace Montr.Docs.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class DocumentType
	{
		private string DebuggerDisplay => $"{Code}, {Name}";

		public static readonly string EntityTypeCode = nameof(DocumentType);

		public Guid Uid { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string Url { get; set; }
	}
}
