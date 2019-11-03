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
		public async Task<ApiResult> Register(Register request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> ExternalRegister(ExternalRegister request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> SendEmailConfirmation(SendEmailConfirmation request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> ConfirmEmail(ConfirmEmail request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Login(Login request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<IList<AuthScheme>> AuthSchemes(GetAuthSchemes request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ExternalLoginCallback.Result> ExternalLoginCallback(ExternalLoginCallback request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Logout(Logout request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> ForgotPassword(ForgotPassword request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> ResetPassword(ResetPassword request)
		{
			return await _mediator.Send(request);
		}
	}
}
