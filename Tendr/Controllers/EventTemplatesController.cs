using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tendr.Models;

namespace Tendr.Controllers
{
	[ApiController, Route("api/[controller]")]
	public class EventTemplatesController : ControllerBase
	{
		[HttpGet]
		public ActionResult<IEnumerable<EventTemplate>> Get()
		{
			return new[]
			{
				new EventTemplate
				{
					EventType = EventType.RequestForProposal,
					Name = "Запрос предложений",
					Description = "Some descriptive description"
				},
				new EventTemplate
				{
					EventType = EventType.Proposal,
					Name = "Предложение"
				}
			};
		}
	}
}