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

			if (viewId == "PrivateEvent/Edit")
			{
				result.Panes = new List<DataPane>
				{
					new DataPane { Key = "tab_info", Name = "Информация", Icon = "profile",
						Component = "panes/private/EditEventPane" },
					new DataPane { Key = "tab_team", Name = "Команда", Icon = "team" },
					new DataPane { Key = "tab_items", Name = "Позиции", Icon = "table" },
					new DataPane { Key = "tab_history", Name = "История изменений", Icon = "eye" },
					new DataPane { Key = "tab_5", Name = "Тендерная комиссия (команда?)" },
					new DataPane { Key = "tab_6", Name = "Критерии оценки (анкета?)" },
					new DataPane { Key = "tab_7", Name = "Документы (поле?)" },
					new DataPane { Key = "tab_8", Name = "Контактные лица (поле?)" },
				};
			}

			return result;
		}
	}
}