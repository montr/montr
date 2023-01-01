using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Metadata.Models;
using Montr.Tasks.Queries;

namespace Montr.Tasks.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class TaskMetadataController : ControllerBase
	{
		private readonly ISender _mediator;

		public TaskMetadataController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<DataView> SearchMetadata(GetTaskSearchMetadata request)
		{
			return await _mediator.Send(request);
		}
	}
}
