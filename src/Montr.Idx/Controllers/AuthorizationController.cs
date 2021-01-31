using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Idx.Commands.Oidc;

namespace Montr.Idx.Controllers
{
	public class AuthorizationController : Controller
	{
		private readonly ISender _mediator;

		public AuthorizationController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("~/connect/authorize")]
		[HttpPost("~/connect/authorize")]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> Authorize()
		{
			return await _mediator.Send(new OidcAuthorize());
		}

		[HttpGet("~/connect/logout")]
		public async Task<IActionResult> Logout()
		{
			return await _mediator.Send(new OidcLogout());
		}
	}
}
