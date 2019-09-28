using MediatR;
using Montr.Core.Models;
using Montr.Tendr.Models;

namespace Montr.Tendr.Queries
{
	public class GetInvitationList : InvitationSearchRequest, IRequest<SearchResult<InvitationListItem>>
	{
	}
}
