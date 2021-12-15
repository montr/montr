using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Queries;

namespace Montr.Core.Controllers;

[Authorize, ApiController, Route("api/[controller]/[action]")]
public class EntityRelationController : ControllerBase
{
	private readonly ISender _mediator;

	public EntityRelationController(ISender mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	public async Task<IList<EntityRelation>> List(GetEntityRelationList request)
	{
		return await _mediator.Send(request);
	}
}
