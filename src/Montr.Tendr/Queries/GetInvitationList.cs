using System;
using MediatR;
using Montr.Core.Models;
using Montr.Tendr.Models;

namespace Montr.Tendr.Queries
{
	public class GetInvitationList : IRequest<SearchResult<InvitationListItem>>
	{
		public Guid UserUid { get; set; }

		public InvitationSearchRequest Request { get; set; }
	}
}
