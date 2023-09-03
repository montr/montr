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

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(Permissions.ViewDocument))]
		public async Task<SearchResult<Document>> List(GetDocumentList request)
		{
			request.Principal = User;
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(Permissions.ViewDocument))]
		public async Task<Document> Get(GetDocument request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(Permissions.SubmitDocument))]
		public async Task<ApiResult> Submit(SubmitDocument request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(Permissions.SubmitDocument))]
		public async Task<ApiResult> ChangeStatus(ChangeDocumentStatus request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(Permissions.SubmitDocument))]
		public async Task<ApiResult> CreateRelated(CreateRelatedDocument request)
		{
			request.Principal = User;
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
