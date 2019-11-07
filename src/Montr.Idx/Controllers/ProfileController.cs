using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Idx.Commands;

namespace Montr.Idx.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class ProfileController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ProfileController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateProfile request)
		{
			return await _mediator.Send(request);
		}
	}
}
