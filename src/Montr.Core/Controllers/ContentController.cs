using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Queries;

namespace Montr.Core.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class ContentController : ControllerBase
	{
		private readonly ISender _mediator;

		public ContentController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<ActionResult<Menu>> Menu(GetMenu request)
		{
			request.Principal = User;

			return await _mediator.Send(request);
		}
	}
}
