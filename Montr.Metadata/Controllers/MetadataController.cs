using Microsoft.AspNetCore.Mvc;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Metadata.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class MetadataController : ControllerBase
	{
		private readonly IMetadataProvider _metadataProvider;

		public MetadataController(IMetadataProvider metadataProvider)
		{
			_metadataProvider = metadataProvider;
		}

		[HttpPost]
		public ActionResult<DataView> View(MetadataRequest request)
		{
			return _metadataProvider.GetView(request.ViewId);
		}
	}
}