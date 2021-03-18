using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Models;
using Montr.Idx.Queries;

namespace Montr.Idx.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class ProfileController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public ProfileController(IMediator mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<ProfileModel> Get(GetProfile request)
		{
			request.User = _currentUserProvider.GetUser();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<IList<ExternalLoginModel>> ExternalLogins(GetExternalLogins request)
		{
			request.User = _currentUserProvider.GetUser();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateProfile request)
		{
			request.User = _currentUserProvider.GetUser();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> ChangeEmail(ChangeEmail request)
		{
			request.User = _currentUserProvider.GetUser();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> ChangePhone(ChangePhone request)
		{
			request.User = _currentUserProvider.GetUser();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> ChangePassword(ChangePassword request)
		{
			request.User = _currentUserProvider.GetUser();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> SetPassword(SetPassword request)
		{
			request.User = _currentUserProvider.GetUser();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> LinkLoginCallback(LinkLoginCallback request)
		{
			request.User = _currentUserProvider.GetUser();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> RemoveLogin(RemoveLogin request)
		{
			request.User = _currentUserProvider.GetUser();

			return await _mediator.Send(request);
		}
	}
}
