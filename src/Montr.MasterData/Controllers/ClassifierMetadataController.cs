using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.MasterData.Permissions;
using Montr.MasterData.Queries;
using Montr.Metadata.Models;

namespace Montr.MasterData.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class ClassifierMetadataController : ControllerBase
	{
		private readonly ISender _mediator;

		public ClassifierMetadataController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpPost, Permission(typeof(ViewClassifiers))]
		public async Task<DataView> View(GetClassifierMetadata request)
		{
			request.Principal = User;

			return await _mediator.Send(request);
		}
	}
}
