using System;
using System.Diagnostics;

namespace Montr.Docs.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class DocumentType
	{
		private string DebuggerDisplay => $"{Code}, {Name}";

		// todo: use settings for active
		public static readonly Guid CompanyRegistrationRequest = Guid.Parse("ab770d9f-f723-4468-8807-5df0f6637cca");

		public static readonly string EntityTypeCode = nameof(DocumentType);

		public Guid? Uid { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string Url { get; set; }
	}
}
