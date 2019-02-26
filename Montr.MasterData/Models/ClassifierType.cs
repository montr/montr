using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class ClassifierType
	{
		public string Code { get; set; }

		public string Name { get; set; }

		public bool IsSystem { get; set; }

		public HierarchyType HierarchyType { get; set; }

		private string DebuggerDisplay => $"{Code}, {Name}";
	}
	
	public enum HierarchyType
	{
		None = 0,
		Folders = 1,
		Items = 2
	}
}
