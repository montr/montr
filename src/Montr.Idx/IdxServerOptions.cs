namespace Montr.Idx
{
	public class IdxServerOptions
	{
		public string PublicOrigin { get; set; }

		// todo: temp solution, read client configuration from db
		public string[] ClientUrls { get; set; }
	}
}
