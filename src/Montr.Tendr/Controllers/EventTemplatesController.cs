using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Kompany.Services;
using Montr.Tendr.Models;
using Montr.Tendr.Queries;
using Montr.Web.Services;

namespace Montr.Tendr.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class EventTemplateController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly ICurrentCompanyProvider _currentCompanyProvider;
		private readonly ICurrentUserProvider _currentUserProvider;

		public EventTemplateController(IMediator mediator,
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
			request.IsTemplate = true;

			return await _mediator.Send(request);
		}
	}
}
