using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Metadata.Models;
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

		[HttpPost]
		public async Task<DataView> Metadata(GetSettingsMetadata request)
		{
			request.Principal = User;

			return await _mediator.Send(request);
		}
	}
}
