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
			return new[]
			{
				new EventTemplate
				{
					Id = System.Guid.NewGuid(),
					EventType = EventType.RequestForProposal,
					Name = "Запрос предложений",
					Description = "Some descriptive description"
				},
				new EventTemplate
				{
					Id = System.Guid.NewGuid(),
					EventType = EventType.Proposal,
					Name = "Предложение",
					Description = "Руки и сердца"
				}
			};
		}
	}
}