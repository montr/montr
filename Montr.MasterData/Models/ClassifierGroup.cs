using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class ClassifierGroup
	{
		private string DebuggerDisplay => $"{Code}, {Name}";

		// todo: remove?
		// public System.Guid Uid { get; set; }

		// public string TypeCode { get; set; }

		public string Code { get; set; }

		public string ParentCode { get; set; }

		public string Name { get; set; }
	}
}