using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Metadata.Models;
using Montr.Tendr.Commands;
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
		public async Task<ActionResult<SearchResult<InvitationListItem>>> List(InvitationSearchRequest request)
		{
			return await _mediator.Send(new GetInvitationList
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Request = request
			});
		}

		[HttpPost]
		public async Task<Invitation> Get(GetInvitation request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Insert(InsertInvitation request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateInvitation request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Delete(DeleteInvitation request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
