using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Core.Controllers
{
	[/* Authorize, */ ApiController, Route("api/[controller]/[action]")]
	public class MetadataController : ControllerBase
	{
		private readonly IMetadataProvider _metadataProvider;

		public MetadataController(IMetadataProvider metadataProvider)
		{
			_metadataProvider = metadataProvider;
		}

		[HttpPost]
		public async Task<DataView> View(MetadataRequest request)
		{
			return await _metadataProvider.GetView(request.ViewId);
		}
	}
}
