using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Automate.Commands;
using Montr.Automate.Models;
using Montr.Automate.Queries;
using Montr.Core.Models;

namespace Montr.Automate.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class AutomationController : ControllerBase
	{
		private readonly IMediator _mediator;

		public AutomationController(IMediator mediator)
		{
			_mediator = mediator;
		}

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
		}

		[HttpPost]
		public async Task<IList<AutomationAction>> ActionTypes()
		{
			return await _mediator.Send(new GetAutomationActionList());
		}

		[HttpPost]
		public async Task<IList<AutomationCondition>> ConditionTypes()
		{
			return await _mediator.Send(new GetAutomationConditionList());
		}
	}
}
