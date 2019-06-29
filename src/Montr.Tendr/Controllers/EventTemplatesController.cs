using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Montr.Tendr.Models;

namespace Montr.Tendr.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class EventTemplatesController : ControllerBase
	{
		[HttpPost]
		public ActionResult<IEnumerable<EventTemplate>> List()
		{
			return EventTemplate.All;
		}
	}
}
