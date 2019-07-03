using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class ClassifierTree
	{
		private string DebuggerDisplay => $"{Code}, {Name}";

		public System.Guid Uid { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }
	}
}
