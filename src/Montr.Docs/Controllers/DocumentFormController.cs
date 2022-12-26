using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Docs.Commands;

namespace Montr.Docs.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class DocumentFormController : ControllerBase
	{
		private readonly ISender _mediator;

		public DocumentFormController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateDocumentForm request)
		{
			return await _mediator.Send(request);
		}
	}
}