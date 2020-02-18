namespace Montr.Core
{
	public class AppOptions
	{
		public string AppUrl { get; set; }

		public string AuthorityAppUrl { get; set; }

		// todo: temp solution, read client configuration from db
		public string[] ClientUrls { get; set; }

		// todo: always auto calc (?)
		public string CookieDomain { get; set; }
	}
}
