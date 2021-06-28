using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Kompany.Registration.Commands;
using Montr.Kompany.Registration.Queries;

namespace Montr.Kompany.Registration.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class CompanyRegistrationController : ControllerBase
	{
		private readonly ISender _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public CompanyRegistrationController(ISender mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		public async Task<ICollection<Document>> Requests(GetCompanyRegistrationRequestList request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		public async Task<ApiResult> CreateRequest()
		{
			return await _mediator.Send(new CreateCompanyRegistrationRequest
			{
				UserUid = _currentUserProvider.GetUserUid()
			});
		}
	}
}
