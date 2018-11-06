using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tendr.Models;

namespace Tendr.Controllers.Apis
{
    [ApiController, Route("api/[controller]/[action]")]
    public class EventsController : ControllerBase
    {
		private static readonly List<Event> _events = new List<Event>();

        [HttpPost]
        public ActionResult<IEnumerable<Event>> Load()
        {
            return _events;
        }

        [HttpPost]
        public ActionResult<bool> Create(Event item)
		{
			item.Id = System.Guid.NewGuid();
			item.EventType = EventType.RequestForProposal;
			item.Number = $"RFP-{_events.Count + 1:D6}";

			_events.Insert(0, item);

            return true;
        }
    }
}