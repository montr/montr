using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc;
using Tendr.Entities;
using Tendr.Models;
using Tendr.Services;

namespace Tendr.Controllers.Apis
{
	[ApiController, Route("api/[controller]/[action]")]
	public class EventsController : ControllerBase
	{
		[HttpPost]
		public ActionResult<DataResult<Event>> Load(EventSearchRequest request)
		{
			if (request.PageNo <= 0)
			{
				request.PageNo = 1;
			}

			if (request.PageSize <= 0 || request.PageSize > 100)
			{
				request.PageSize = 10;
			}

			if (request.SortColumn == null)
			{
				request.SortColumn = "Id";
			}

			using (var db = new DbContext())
			{
				var all = db.GetTable<DbEvent>();

				var ordered =
					request.SortOrder == SortOrder.Ascending
						? all.OrderBy(request.SortColumn)
						: all.OrderByDescending(request.SortColumn);

				var paged = ordered
					.Skip((request.PageNo - 1) * request.PageSize)
					.Take(request.PageSize);

				var date = paged
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