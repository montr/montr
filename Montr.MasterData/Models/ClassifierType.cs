using System;
using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class ClassifierType
	{
		private string DebuggerDisplay => $"{Code}, {Name}";

		public Guid Uid { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }

		public bool IsSystem { get; set; }

		public HierarchyType HierarchyType { get; set; }
	}
	
	public enum HierarchyType
	{
		None = 0,
		Groups = 1,
		Items = 2
	}
}
