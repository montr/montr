using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Classifier
	{
		public System.Guid Uid { get; set; }

		public string ConfigCode { get; set; }

		public string StatusCode { get; set; }

		// public System.Guid CompanyUid { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }

		private string DebuggerDisplay => $"{Code}, {Name}";
	}
}
