namespace Montr.Core.Models
{
	public class LocaleStringSearchRequest : SearchRequest
	{
		public string Locale { get; set; }

		public string Module { get; set; }
	}
}
