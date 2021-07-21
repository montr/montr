using System.Security.Claims;

namespace Montr.Metadata.Models
{
	public class MetadataRequest
	{
		public ClaimsPrincipal Principal { get; set; }

		public string ViewId { get; set; }
	}
}
