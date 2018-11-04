using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tendr.Models;

namespace Tendr.Controllers.Apis
{
	[ApiController, Route("api/[controller]")]
	public class EventsController : ControllerBase
	{
		[HttpGet]
		public ActionResult<IEnumerable<Event>> Get()
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