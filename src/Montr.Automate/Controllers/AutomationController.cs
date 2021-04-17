﻿using System.Collections.Generic;
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
	[Authorize(), ApiController, Route("api/[controller]/[action]")]
	public class AutomationController : ControllerBase
	{
		private readonly ISender _mediator;

		public AutomationController(ISender mediator)
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
		public async Task<IList<AutomationRuleType>> ActionTypes()
		{
			return await _mediator.Send(new GetAutomationActionTypeList());
		}

		[HttpPost]
		public async Task<IList<AutomationRuleType>> ConditionTypes()
		{
			return await _mediator.Send(new GetAutomationConditionTypeList());
		}
	}
}
