using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Kompany.Commands;

namespace Montr.Kompany.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class SetupController : Controller
	{
		private readonly IMediator _mediator;

		public SetupController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<ApiResult> Save(SetupApplication request)
		{
			return await _mediator.Send(request);
		}
	}
}
