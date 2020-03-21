using System;
using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Numerator
	{
		private string DebuggerDisplay => $"{Name} - {Pattern}";

		public static readonly string DefaultPattern = "{Number}";

		public Guid Uid { get; set; }

		public string Name { get; set; }

		public string Pattern { get; set; }

		public string[] KeyTags { get; set; }

		public NumeratorPeriodicity Periodicity { get; set; }

		public bool IsActive { get; set; }

		public bool IsSystem { get; set; }
	}

	public class NumeratorKnownTags
	{
		public static readonly string Number = "{Number}";

		public static readonly string Day = "{Day}";

		public static readonly string Month = "{Month}";

		public static readonly string Quarter = "{Quarter}";

		public static readonly string Year2 = "{Year2}";

		public static readonly string Year4 = "{Year4}";
	}

	public enum NumeratorPeriodicity
	{
		None = 0,
		Day = 1,
		// Week = 2, // requires settings for start of week - Monday or Sunday
		Month = 3,
		Quarter = 4,
		Year = 5
	}
}
