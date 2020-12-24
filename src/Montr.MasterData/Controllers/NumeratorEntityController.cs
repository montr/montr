using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class NumeratorEntityController : ControllerBase
	{
		private readonly IMediator _mediator;

		public NumeratorEntityController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<SearchResult<NumeratorEntity>> List(GetNumeratorEntityList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<NumeratorEntity> Get(GetNumeratorEntity request)
		{
			return await _mediator.Send(request);
		}
	}
}
