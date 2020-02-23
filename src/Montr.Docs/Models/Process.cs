using System;
using System.Diagnostics;

namespace Montr.Docs.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Process
	{
		public static readonly Guid Registration = Guid.Parse("8C41EBDC-E176-424E-9048-249E9862DBB2");

		private string DebuggerDisplay => $"{Code}, {Name}";

		public static readonly string EntityTypeCode = typeof(Process).Name;

		public Guid Uid { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }
	}
}
