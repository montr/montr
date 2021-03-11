using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Models;
using Montr.Idx.Queries;

namespace Montr.Idx.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class RoleController : ControllerBase
	{
		private readonly IMediator _mediator;

		public RoleController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<SearchResult<Role>> List(GetRoleList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<Role> Create(CreateRole request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<Role> Get(GetRole request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Insert(InsertRole request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateRole request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Delete(DeleteRole request)
		{
			return await _mediator.Send(request);
		}
	}
}
