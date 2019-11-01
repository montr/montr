using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Idx.Commands;

namespace Montr.Idx.Controllers
{
	[Route("api/[controller]/[action]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly IMediator _mediator;

		public AuthenticationController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<ChallengeResult> ExternalLogin(ExternalLoginCommand request)
		{
			return await _mediator.Send(request);
		}

		// [HttpPost]
		public async Task<IActionResult> ExternalLoginCallback(ExternalLoginCallbackCommand request)
		{
			return await _mediator.Send(request);
		}
	}
}
