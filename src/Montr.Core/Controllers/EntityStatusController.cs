using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Commands;
using Montr.Core.Models;
using Montr.Core.Queries;

namespace Montr.Core.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class EntityStatusController : ControllerBase
	{
		private readonly IMediator _mediator;

		public EntityStatusController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<SearchResult<EntityStatus>> List(GetEntityStatusList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<EntityStatus> Get(GetEntityStatus request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Insert(InsertEntityStatus request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateEntityStatus request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Delete(DeleteEntityStatus request)
		{
			return await _mediator.Send(request);
		}
	}
}
