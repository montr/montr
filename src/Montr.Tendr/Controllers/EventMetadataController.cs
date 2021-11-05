using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Metadata.Models;
using Montr.Tendr.Queries;

namespace Montr.Tendr.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class EventMetadataController : ControllerBase
	{
		private readonly ISender _mediator;

		public EventMetadataController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<DataView> View(GetEventMetadata request)
		{
			request.Principal = User;

			return await _mediator.Send(request);
		}
	}
}
