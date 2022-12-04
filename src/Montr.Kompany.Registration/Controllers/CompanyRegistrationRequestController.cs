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
	public class CompanyRegistrationRequestController : ControllerBase
	{
		private readonly ISender _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public CompanyRegistrationRequestController(ISender mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		public async Task<ICollection<Document>> Search(GetCompanyRegistrationRequestList request)
		{
			request.UserUid = _currentUserProvider.GetUserUidIfAuthenticated();

			return await _mediator.Send(request);
		}

		public async Task<ApiResult> Create(CreateCompanyRegistrationRequest request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		public async Task<ApiResult> Delete(DeleteCompanyRegistrationRequest request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Submit(SubmitCompanyRegistrationRequest request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
