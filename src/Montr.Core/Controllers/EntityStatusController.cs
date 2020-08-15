using Microsoft.AspNetCore.Mvc;

namespace Montr.Core.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class EntityStatusController : ControllerBase
	{
		[HttpPost]
		public async Task<SearchResult<Automation>> List(GetAutomationList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<Automation> Get(GetAutomation request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Insert(InsertAutomation request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateAutomation request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Delete(DeleteAutomation request)
		{
			return await _mediator.Send(request);
		}}
}
