using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Docs.Commands;
using Montr.Docs.Models;
using Montr.Docs.Queries;

namespace Montr.Docs.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class ProcessStepController : ControllerBase
	{
		private readonly ISender _mediator;

		public ProcessStepController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<SearchResult<ProcessStep>> List(GetProcessStepList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Insert(InsertProcessStep request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateProcessStep request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Delete(DeleteProcessStep request)
		{
			return await _mediator.Send(request);
		}
	}
}