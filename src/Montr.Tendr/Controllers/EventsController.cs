using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Kompany.Services;
using Montr.Tendr.Commands;
using Montr.Tendr.Models;
using Montr.Tendr.Queries;

namespace Montr.Tendr.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class EventsController : ControllerBase
	{
		private readonly ISender _mediator;
		private readonly ICurrentCompanyProvider _currentCompanyProvider;
		private readonly ICurrentUserProvider _currentUserProvider;

		public EventsController(ISender mediator,
			ICurrentCompanyProvider currentCompanyProvider, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentCompanyProvider = currentCompanyProvider;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<SearchResult<Event>> List(GetEventList request)
		{
			request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.UserUid = _currentUserProvider.GetUserUid();

			request.IsTemplate = false;

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<Event> Get(GetEvent request)
		{
			request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Insert(InsertEvent request)
		{
			request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateEvent request)
		{
			request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Publish(PublishEvent request)
		{
			request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Cancel(CancelEvent request)
		{
			request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
