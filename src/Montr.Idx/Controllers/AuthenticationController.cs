using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Idx.Commands;
using Montr.Idx.Services;

namespace Montr.Idx.Controllers
{
	[Route("api/[controller]/[action]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly ISender _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public AuthenticationController(ISender mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<ChallengeResult> ExternalLogin(ExternalLogin request)
		{
			return await _mediator.Send(request);
		}

		[Authorize, HttpPost]
		public async Task<ChallengeResult> LinkLogin(LinkLogin request)
		{
			request.User = _currentUserProvider.GetUser();

			return await _mediator.Send(request);
		}
	}
}
