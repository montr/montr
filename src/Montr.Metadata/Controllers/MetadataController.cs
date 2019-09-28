using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Metadata.Controllers
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
