using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Data.Linq2Db;
using Montr.Metadata.Models;
using Montr.Metadata.Services;
using Montrl.Web.Models;
using Tendr.Entities;
using Tendr.Models;

namespace Tendr.Controllers
{
	[/*Authorize,*/ ApiController, Route("api/[controller]/[action]")]
	public class EventsController : ControllerBase
	{
		[HttpPost]
		public ActionResult<DataResult<Event>> Load(EventSearchRequest request)
		{
			using (var db = new DbContext())
			{
				var all = db.GetTable<DbEvent>();

				var date = all
					.Apply(request, "Id", SortOrder.Descending)
					.Select(x => new Event
					{
						Uid = x.Uid,
						Id = x.Id,
						ConfigCode = x.ConfigCode,
						StatusCode = x.StatusCode,
						Name = x.Name,
						Description = x.Description,
						Url = "/events/edit/" + x.Id
					})
					.ToList();

				return new DataResult<Event>
				{
					TotalCount = all.Count(),
					Rows = date
				};
			}
		}

		[HttpPost]
		public ActionResult<Event> Get(Event item) // todo: pass only id
		{
			using (var db = new DbContext())
			{
				var result = db.GetTable<DbEvent>()
					.Where(x => x.Id == item.Id)
					.Select(x => new Event
					{
						Uid = x.Uid,
						Id = x.Id,
						ConfigCode = x.ConfigCode,
						StatusCode = x.StatusCode,
						Name = x.Name,
						Description = x.Description,
						Url = "/events/edit/" + x.Id
					})
					.Single();

				return result;
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

		[HttpPost]
		public ActionResult<ApiResult> Update(Event item)
		{
			using (var db = new DbContext())
			{
				db.GetTable<DbEvent>()
					.Where(x => x.Id == item.Id)
					.Set(x => x.Name, item.Name)
					.Set(x => x.Description, item.Description)
					.Update();

				return new ApiResult();
			}
		}

		[HttpPost]
		public ActionResult<ApiResult> Publish(Event item) // todo: pass only id
		{
			using (var db = new DbContext())
			{
				var affected = db.GetTable<DbEvent>()
					.Where(x => x.Id == item.Id)
					.Set(x => x.StatusCode, "published")
					.Update();

				return new ApiResult { Success = (affected == 1) };
			}
		}

		[HttpPost]
		public ActionResult<ApiResult> Cancel(Event item) // todo: pass only id
		{
			using (var db = new DbContext())
			{
				var affected = db.GetTable<DbEvent>()
					.Where(x => x.Id == item.Id)
					.Set(x => x.StatusCode, "draft") // "cancelled"
					.Update();

				return new ApiResult { Success = (affected == 1) };
			}
		}
	}
}