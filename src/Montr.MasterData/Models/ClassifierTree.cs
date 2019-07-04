using System;
using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class ClassifierTree
	{
		public const string DefaultCode = "default";

		private string DebuggerDisplay => $"{Code}, {Name}";

		public Guid Uid { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }
	}
}
