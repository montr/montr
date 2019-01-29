using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Metadata.Models;
using Montr.Web.Services;
using Tendr.Commands;
using Tendr.Queries;
using Tendr.Models;

namespace Tendr.Web.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class EventsController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public EventsController(IMediator mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<ActionResult<DataResult<Event>>> Load(EventSearchRequest request)
		{
			return await _mediator.Send(new LoadEventList
			{
				Request = request
			});

			/*using (var db = new DbContext())
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
			}*/
		}

		[HttpPost]
		public async Task<ActionResult<Event>> Get(Event item) // todo: pass only id
		{
			return await _mediator.Send(new GetEvent
			{
				EventId = item.Id
			});

			/*using (var db = new DbContext())
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
			}*/
		}

		[HttpPost]
		public async Task<ActionResult<long>> Create(Event item)
		{
			return await _mediator.Send(new CreateEvent
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Event = item
			});
		}

		[HttpPost]
		public async Task<ActionResult> Update(Event item)
		{
			await _mediator.Send(new UpdateEvent
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Event = item
			});

			return Ok();

			/*using (var db = new DbContext())
			{
				db.GetTable<DbEvent>()
					.Where(x => x.Id == item.Id)
					.Set(x => x.Name, item.Name)
					.Set(x => x.Description, item.Description)
					.Update();

				return new ApiResult();
			}*/
		}

		[HttpPost]
		public async Task<ActionResult> Publish(Event item) // todo: pass only id
		{
			await _mediator.Send(new PublishEvent
			{
				UserUid = _currentUserProvider.GetUserUid(),
				EventId = item.Id
			});

			return Ok();

			/*using (var db = new DbContext())
			{
				var affected = db.GetTable<DbEvent>()
					.Where(x => x.Id == item.Id)
					.Set(x => x.StatusCode, EventStatusCode.Published)
					.Update();

				return new ApiResult { Success = (affected == 1) };
			}*/
		}

		[HttpPost]
		public async Task<ActionResult> Cancel(Event item) // todo: pass only id
		{
			await _mediator.Send(new CancelEvent
			{
				UserUid = _currentUserProvider.GetUserUid(),
				EventId = item.Id
			});

			return Ok();

			/*using (var db = new DbContext())
			{
				var affected = db.GetTable<DbEvent>()
					.Where(x => x.Id == item.Id)
					.Set(x => x.StatusCode, EventStatusCode.Draft) // "cancelled"
					.Update();

				return new ApiResult { Success = (affected == 1) };
			}*/
		}
	}
}
