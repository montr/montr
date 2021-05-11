using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Queries;

namespace Montr.Idx.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class RolePermissionController : ControllerBase
	{
		private readonly ISender _mediator;

		public RolePermissionController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpPost, Permission(typeof(Permissions.ViewRolePermissions))]
		public async Task<SearchResult<Permission>> List(GetRolePermissions request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(Permissions.ManageRolePermissions))]
		public async Task<ApiResult> Update(ManageRolePermissions request)
		{
			return await _mediator.Send(request);
		}
	}
}
