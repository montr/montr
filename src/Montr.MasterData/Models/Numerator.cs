using System;
using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Numerator
	{
		private string DebuggerDisplay => $"{Name} - {Pattern}";
		public Guid Uid { get; set; }

		public string Name { get; set; }

		public string Pattern { get; set; }

		public Periodicity Periodicity { get; set; }

		public bool IsActive { get; set; }

		public bool IsSystem { get; set; }
	}

	public enum Periodicity
	{
		None = 0,
		Day = 1,
		Week = 2,
		Month = 3,
		Quarter = 4,
		Year = 5
	}
}
