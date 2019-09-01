using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Tendr.Models;
using Montr.Tendr.Queries;
using Montr.Web.Services;

namespace Montr.Tendr.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class InvitationController
	{
		private readonly IMediator _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public InvitationController(IMediator mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<ActionResult<SearchResult<Invitation>>> List(InvitationSearchRequest request)
		{
			return await _mediator.Send(new GetInvitationList
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Request = request
			});
		}
	}
}