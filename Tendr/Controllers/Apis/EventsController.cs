using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tendr.Models;

namespace Tendr.Controllers.Apis
{
	[ApiController, Route("api/[controller]/[action]")]
	public class EventsController : ControllerBase
	{
		[HttpPost]
		public ActionResult<IEnumerable<Event>> Load()
		{
			var result = new List<Event>();

			for (var i = 1; i < 1000; i++)
			{
				result.Add(new Event
				{
					Id = System.Guid.NewGuid(),
					EventType = EventType.RequestForProposal,
					Number = string.Format("T-{0:D8}", i),
					Name = "Запрос предложений",
					Description = "Mandatory bandwidth-monitored collaboration"
				});
			}

			return result;
		}
	}
}