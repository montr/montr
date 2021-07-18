using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Montr.Tendr.Queries;

namespace Montr.Tendr.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class EventMetadataController : ControllerBase
	{
		private readonly ISender _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public EventMetadataController(ISender mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<DataView> View(GetEventMetadata request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
