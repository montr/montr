using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Kompany.Commands;

namespace Montr.Kompany.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class SetupController : ControllerBase
	{
		private readonly ISender _mediator;

		public SetupController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<ApiResult> Save(SetupSystem request)
		{
			return await _mediator.Send(request);
		}
	}
}
