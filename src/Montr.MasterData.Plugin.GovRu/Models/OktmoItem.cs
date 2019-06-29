using System;

namespace Montr.MasterData.Plugin.GovRu.Models
{
	public class OktmoItem : OkItem
	{
		public DateTime? StartDateActive { get; set; }

		// for CSV columns

		public string Code1 { get; set; }

		public string Code2 { get; set; }

		public string Code3 { get; set; }

		public string Code4 { get; set; }

		public string ControlNo { get; set; }

		public string SectionCode { get; set; }

		public string AdditionalInfo { get; set; }

		public string Description { get; set; }

		public int ActNo { get; set; }

		public int StatusNo { get; set; }

		public DateTime? DateAccepted { get; set; }
	}
}
