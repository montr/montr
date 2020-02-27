using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Docs.Models;
using Montr.Docs.Queries;

namespace Montr.Docs.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class DocumentController : ControllerBase
	{
		private readonly IMediator _mediator;

		public DocumentController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<SearchResult<Document>> List(GetDocumentList request)
		{
			return await _mediator.Send(request);
		}
	}
}
