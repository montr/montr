using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Metadata.Models;
using Montr.Tendr.Commands;
using Montr.Tendr.Models;
using Montr.Tendr.Queries;
using Montr.Web.Services;

namespace Montr.Tendr.Controllers
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
		public async Task<ActionResult<SearchResult<Event>>> List(EventSearchRequest request)
		{
			return await _mediator.Send(new GetEventList
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Request = request
			});
		}

		[HttpPost]
		public async Task<ActionResult<Event>> Get(Event item) // todo: pass only id
		{
			return await _mediator.Send(new GetEvent
			{
				Uid = item.Uid
			});
		}

		[HttpPost]
		public async Task<ApiResult> Insert(Event item)
		{
			return await _mediator.Send(new InsertEvent
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Event = item
			});
		}

		[HttpPost]
		public async Task<ApiResult> Update(Event item)
		{
			return await _mediator.Send(new UpdateEvent
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Item = item
			});
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
		}
	}
}
