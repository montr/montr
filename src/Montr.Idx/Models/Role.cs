using System;
using System.Diagnostics;

namespace Montr.Idx.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Role // : Classifier
	{
		private string DebuggerDisplay => $"{Name}";

		public Guid? Uid { get; set; }

		public string Name { get; set; }

		public string ConcurrencyStamp { get; set; }

		public string Url { get; set; }
	}
}
