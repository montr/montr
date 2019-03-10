using System.Collections.Generic;
using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class ClassifierGroup
	{
		private string DebuggerDisplay => $"{Code}, {Name}";

		public string Code { get; set; }

		public string ParentCode { get; set; }

		public string Name { get; set; }

		public IList<ClassifierGroup> Children { get; set; }
	}
}
