using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Kompany.Commands;
using Montr.Kompany.Models;
using Montr.Kompany.Queries;

namespace Montr.Kompany.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class UserCompanyController : ControllerBase
	{
		private readonly ISender _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public UserCompanyController(ISender mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<ICollection<Company>> List(GetUserCompanyList request)
		{
			// todo: remove hack
			if (_currentUserProvider.GetUser(false) != null)
			{
				request.UserUid = _currentUserProvider.GetUserUid();
			}

			return await _mediator.Send(request);
		}

		[Authorize, HttpPost]
		public async Task<ActionResult<ApiResult>> Create(CreateCompany request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
