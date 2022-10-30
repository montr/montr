using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Automate.Commands;
using Montr.Automate.Models;
using Montr.Automate.Permissions;
using Montr.Automate.Queries;
using Montr.Core.Models;
using Montr.Metadata.Models;

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
		public async Task<IList<FieldMetadata>> ActionMetadata(GetAutomationActionMetadata request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<IList<FieldMetadata>> ConditionMetadata(GetAutomationConditionMetadata request)
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

		[HttpPost, Permission(typeof(ManageAutomationRules))]
		public async Task<ApiResult> UpdateRules(UpdateAutomationRules request)
		{
			return await _mediator.Send(request);
		}
	}
}
