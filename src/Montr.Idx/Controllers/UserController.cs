using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Idx.Models;
using Montr.Idx.Queries;
using Montr.Idx.Services;

namespace Montr.Idx.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class UserController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public UserController(IMediator mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<SearchResult<User>> List(GetUserList request)
		{
			// request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.CurrentUserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
