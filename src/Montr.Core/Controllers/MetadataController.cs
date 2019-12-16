using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;

namespace Montr.Core.Controllers
{
	[/* Authorize, */ ApiController, Route("api/[controller]/[action]")]
	public class MetadataController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly IMetadataProvider _metadataProvider;

		public MetadataController(IMediator mediator, IMetadataProvider metadataProvider)
		{
			_mediator = mediator;
			_metadataProvider = metadataProvider;
		}

		[HttpPost]
		public async Task<DataView> View(MetadataRequest request)
		{
			return await _metadataProvider.GetView(request.ViewId);
		}

		[HttpPost]
		public async Task<SearchResult<FormField>> List(GetMetadataList request)
		{
			return await _mediator.Send(request);
		}
	}
}
