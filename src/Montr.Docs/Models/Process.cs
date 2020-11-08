using System;
using System.Diagnostics;

namespace Montr.Docs.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Process
	{
		private string DebuggerDisplay => $"{Code}, {Name}";

		public static readonly string TypeCode = nameof(Process);

		public Guid Uid { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }
	}
}
