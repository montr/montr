namespace Montr.MasterData.Models
{
	public class ClassifierLink
	{
		public ClassifierGroup Group { get; set; }

		public Classifier Item { get; set; }

		// todo: use Group.Code
		public string GroupCode { get; set; }

		// todo: use Item.Code
		public string ItemCode { get; set; }

		// todo: use Item.StatusCode
		public string ItemStatusCode { get; set; }
	}
}
