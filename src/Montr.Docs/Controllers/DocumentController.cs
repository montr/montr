using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Docs.Commands;
using Montr.Docs.Models;
using Montr.Docs.Queries;
using Montr.Metadata.Models;

namespace Montr.Docs.Controllers;

[Authorize, ApiController, Route("api/[controller]/[action]")]
public class DocumentController : ControllerBase
{
	private readonly ISender _mediator;

	public DocumentController(ISender mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	public async Task<DataView> Metadata(GetDocumentMetadata request)
	{
		request.Principal = User;

		return await _mediator.Send(request);
	}

	[HttpPost]
	public async Task<SearchResult<Document>> List(GetDocumentList request)
	{
		return await _mediator.Send(request);
	}

	[HttpPost]
	public async Task<Document> Get(GetDocument request)
	{
		return await _mediator.Send(request);
	}

	[HttpPost]
	public async Task<ApiResult> Submit(SubmitDocument request)
	{
		return await _mediator.Send(request);
	}
}