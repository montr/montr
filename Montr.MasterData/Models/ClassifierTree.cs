using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class ClassifierTree
	{
		public const string DefaultTreeCode = "default";

		private string DebuggerDisplay => $"{Code}, {Name}";

		// todo: remove?
		// public System.Guid Uid { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }
	}
}
