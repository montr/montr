using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Montr.Core.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class AppController : ControllerBase
	{
		private readonly IOptionsMonitor<AppOptions> _optionsAccessor;

		public AppController(IOptionsMonitor<AppOptions> optionsAccessor)
		{
			_optionsAccessor = optionsAccessor;
		}

		[HttpPost]
		public AppOptions Options()
		{
			return _optionsAccessor.CurrentValue;
		}
	}
}
