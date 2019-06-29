using System;

namespace Montr.MasterData.Plugin.GovRu.Models
{
	public class OkItem
	{
		public Guid? Uid { get; set; }

		public string BusinessStatus { get; set; }

		public DateTime? ChangeDateTime { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }

		public string ParentCode { get; set; }
	}
}
