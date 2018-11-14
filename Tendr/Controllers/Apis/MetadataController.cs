using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tendr.Models;
using Tendr.Service;

namespace Tendr.Controllers.Apis
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