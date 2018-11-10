using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc;
using Tendr.Entities;
using Tendr.Models;

namespace Tendr.Controllers.Apis
{
    [ApiController, Route("api/[controller]/[action]")]
    public class EventsController : ControllerBase
    {
        [HttpPost]
        public ActionResult<IEnumerable<Event>> Load()
        {
			using (var db = new DbContext())
			{
				return db.GetTable<DbEvent>()
					.OrderByDescending(x => x.Id)
					.Select(x => new Event
					{
						Uid = x.Uid,
						Id = x.Id,
						ConfigCode = x.ConfigCode,
						StatusCode = x.StatusCode,
						Name = x.Name,
						Description = x.Description
					}).ToList();
			}
        }

        [HttpPost]
        public ActionResult<long> Create(Event item)
		{
			using (var db = new DbContext())
			{
				var id = db.Execute<long>("select nextval('event_id_seq')");

				db.GetTable<DbEvent>()
					.Value(x => x.Id, id)
					.Value(x => x.ConfigCode, item.ConfigCode)
					.Value(x => x.StatusCode, "draft")
					.Value(x => x.Name, item.Name)
					.Value(x => x.Description, item.Description)
					.Insert();

				return id;
			}
        }
    }
}