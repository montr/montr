using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Docs.Queries;
using Montr.Metadata.Models;

namespace Montr.Docs.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class DocumentMetadataController : ControllerBase
	{
		private readonly ISender _mediator;

		public DocumentMetadataController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<DataView> View(GetDocumentMetadata request)
		{
			return await _mediator.Send(request);
		}
	}
}
