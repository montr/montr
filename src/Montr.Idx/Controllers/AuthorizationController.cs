using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

		[HttpGet("~/connect/authorize"), HttpPost("~/connect/authorize")]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> Authorize()
		{
			return await _mediator.Send(new OidcAuthorize());
		}

		[Authorize(/*Constants.OpenIdServerAuthenticationScheme*/)]
		[HttpGet("~/connect/userinfo"), HttpPost("~/connect/userinfo")]
		public async Task<IActionResult> Userinfo()
		{
			return await _mediator.Send(new OidcUserInfo());
		}

		[HttpGet("~/connect/logout")]
		public async Task<IActionResult> Logout()
		{
			return await _mediator.Send(new OidcLogout());
		}
	}
}
