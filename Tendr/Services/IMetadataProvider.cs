using System.Collections.Generic;
using Tendr.Models;

namespace Tendr.Service
{
	public interface IMetadataProvider
	{
		DataView GetView(string viewId);
	}

	public class DefaultMetadataProvider : IMetadataProvider
	{
		public DataView GetView(string viewId)
		{
			var result = new DataView { Id = viewId };

			if (viewId == "PrivateEventSearch/Grid")
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "id", Name = "Номер", Sortable = true, Width = 50,
						UrlProperty = "url", DefaultSortOrder = SortOrder.Descending },
					new DataColumn { Key = "configCode", Name = "Тип", Width = 60 },
					new DataColumn { Key = "statusCode", Name = "Статус", Width = 60, Align = DataColumnAlign.Center },
					new DataColumn { Key = "name", Name = "Наименование", Sortable = true, Width = 200, UrlProperty = "url" },
					new DataColumn { Key = "description", Name = "Описание", Width = 300 },
				};
			}

			return result;
		}
	}
}