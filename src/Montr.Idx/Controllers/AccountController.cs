using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Models;
using Montr.Idx.Queries;

namespace Montr.Idx.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class AccountController : ControllerBase
	{
		private readonly IMediator _mediator;

		public AccountController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<ApiResult> Register(RegisterUserCommand request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> SendEmailConfirmation(SendEmailConfirmationCommand request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> ConfirmEmail(ConfirmEmailCommand request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Login(LoginCommand request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<IList<AuthScheme>> AuthSchemes(GetAuthSchemesQuery request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> ExternalLoginCallback(ExternalLoginCallbackCommand request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Logout(LogoutCommand request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> ForgotPassword(ForgotPasswordCommand request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> ResetPassword(ResetPasswordCommand request)
		{
			return await _mediator.Send(request);
		}
	}
}
