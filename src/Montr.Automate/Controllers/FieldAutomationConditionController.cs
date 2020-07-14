using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Automate.Queries;
using Montr.Metadata.Models;

namespace Montr.Automate.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class FieldAutomationConditionController : ControllerBase
	{
		private readonly IMediator _mediator;

		public FieldAutomationConditionController(IMediator mediator)
		{
			_mediator = mediator;
		}
		
		[HttpPost]
		public async Task<IList<FieldMetadata>> Fields(GetFieldAutomationConditionFieldList request)
		{
			return await _mediator.Send(request);
		}
	}
}
