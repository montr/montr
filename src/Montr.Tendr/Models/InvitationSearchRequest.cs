using Montr.Core.Models;

namespace Montr.Tendr.Models
{
	public class InvitationSearchRequest : Paging
	{
		public System.Guid EventUid { get; set; }
	}
}