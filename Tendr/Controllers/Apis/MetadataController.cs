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
					new DataColumn { Key = "id", Name = "Номер", Sortable = true, Width = 40 },
					new DataColumn { Key = "configCode", Name = "Тип", Width = 60 },
					new DataColumn { Key = "statusCode", Name = "Статус", Width = 60 },
					new DataColumn { Key = "name", Name = "Наименование", Width = 200 },
					new DataColumn { Key = "description", Name = "Описание", Width = 300 }
				};
			}

			return Array.Empty<DataColumn>();
		}
    }
}