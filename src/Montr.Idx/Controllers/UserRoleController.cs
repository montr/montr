﻿using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Idx.Models;
using Montr.Idx.Queries;

namespace Montr.Idx.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class UserRoleController : ControllerBase
	{
		private readonly IMediator _mediator;

		public UserRoleController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<SearchResult<Role>> ListRoles(GetUserRoles request)
		{
			return await _mediator.Send(request);
		}
	}
}