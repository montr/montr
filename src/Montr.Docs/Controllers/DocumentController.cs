using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Commands;
using Montr.Docs.Models;
using Montr.Docs.Queries;
using Montr.Metadata.Models;

namespace Montr.Docs.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class DocumentController : ControllerBase
	{
		private readonly ISender _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public DocumentController(ISender mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<DataView> Metadata(GetDocumentMetadata request)
		{
			request.Principal = User;
			request.UserUid = _currentUserProvider.GetUserUid();

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
		public async Task<ApiResult> Publish(PublishDocument request)
		{
			return await _mediator.Send(request);
		}
	}
}
