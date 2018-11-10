using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tendr.Models;

namespace Tendr.Controllers.Apis
{
	[ApiController, Route("api/[controller]/[action]")]
	public class EventTemplatesController : ControllerBase
	{
		[HttpPost]
		public ActionResult<IEnumerable<EventTemplate>> Load()
		{
			return EventTemplate.All;
		}
	}
}