using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Settings.Commands;
using Montr.Settings.Models;
using Montr.Settings.Permissions;
using Montr.Settings.Queries;

namespace Montr.Settings.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class SettingsController : ControllerBase
	{
		private readonly ISender _mediator;

		public SettingsController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpPost, Permission(typeof(ViewSettings))]
		public async Task<ICollection<SettingsBlock>> Metadata(GetSettingsMetadata request)
		{
			request.Principal = User;

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ViewSettings))]
		public async Task<ApiResult<object>> Get(GetSettings request)
		{
			request.Principal = User;

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageSettings))]
		public async Task<ApiResult> Update(UpdateSettings request)
		{
			if (ModelState.IsValid == false) return ModelState.ToApiResult();

			request.Principal = User;

			return await _mediator.Send(request);
		}
	}
}
