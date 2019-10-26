using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Idx.Commands;

namespace Montr.Idx.Controllers
{
	[/*Authorize,*/ ApiController, Route("api/[controller]/[action]")]
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
		public async Task<ApiResult> ConfirmEmail(ConfirmEmailCommand request)
		{
			return await _mediator.Send(request);
		}
	}
}
