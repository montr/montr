using System;

namespace Montr.MasterData.Models
{
	public class ClassifierLink
	{
		public string GroupCode { get; set; }

		public Guid? GroupUid { get; set; }

		public string ItemCode { get; set; }

		public Guid? ItemUid { get; set; }

		// todo: use Classifier.StatusCode
		public string ItemStatusCode { get; set; }
	}
}
