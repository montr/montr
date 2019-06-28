using System;
using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Classifier
	{
		private string DebuggerDisplay => $"{Code}, {Name}";

		public Guid? Uid { get; set; }

		public string StatusCode { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }

		public Guid? ParentUid { get; set; }

		public string ParentCode { get; set; }

		public string Url { get; set; }
	}
}
