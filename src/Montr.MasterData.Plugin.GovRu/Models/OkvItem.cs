using System;

namespace Montr.MasterData.Plugin.GovRu.Models
{
	public class OkvItem : OkItem
	{
		public DateTime? StartDateActive { get; set; }

		public DateTime? EndDateActive { get; set; }

		public int DigitalCode { get; set; }

		public string ShortName { get; set; }
	}
}
