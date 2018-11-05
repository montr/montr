using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tendr.Models;

namespace Tendr.Controllers.Apis
{
	[ApiController, Route("api/[controller]/[action]")]
    public class MetadataController : ControllerBase
    {
		[HttpPost]
		public ActionResult<IEnumerable<DataColumn>> Columns(MetadataRequest request)
		{
			if (request != null && request.ViewId == "PrivateEventSearch/Grid")
			{
				return new List<DataColumn>
				{
					new DataColumn { Key = "number", Name = "Номер", Sortable = true, Width = 100 },
					new DataColumn { Key = "eventType", Name = "Тип", Align = DataColumnAlign.Right, Width = 60 },
					new DataColumn { Key = "name", Name = "Наименование" },
					new DataColumn { Key = "description", Name = "Описание" }
				};
			}

			return Array.Empty<DataColumn>();
		}
    }
}