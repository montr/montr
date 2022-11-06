using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Montr.Settings.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class SettingsController
	{
		private readonly ISender _mediator;

		public SettingsController(ISender mediator)
		{
			_mediator = mediator;
		}
	}
}
