using System;
using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Classifier
	{
		private string DebuggerDisplay => $"{Code}, {Name}";

		// todo: remove?
		public Guid Uid { get; set; }

		public string StatusCode { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }
	}
}
