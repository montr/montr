using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class NumeratorController : ControllerBase
	{
		private readonly IMediator _mediator;

		public NumeratorController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<SearchResult<Numerator>> List(GetNumeratorList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<Numerator> Create(CreateNumerator request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<Numerator> Get(GetNumerator request)
		{
			return await _mediator.Send(request);
		}
	}
}
