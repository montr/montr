using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Services;
using Montr.Docs.Queries;
using Montr.Metadata.Models;

namespace Montr.Docs.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class DocumentMetadataController : ControllerBase
	{
		private readonly ISender _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public DocumentMetadataController(ISender mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<DataView> View(GetDocumentMetadata request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
