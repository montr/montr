using System.Collections.Generic;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	public interface IMetadataProvider
	{
		Task<DataView> GetView(string viewId);
	}

	public class DefaultMetadataProvider : IMetadataProvider
	{
		public async Task<DataView> GetView(string viewId)
		{
			var result = new DataView { Id = viewId };

			if (viewId == "PrivateEventSearch/Grid")
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "id", Name = "Номер", Sortable = true, Width = 10,
						UrlProperty = "url", DefaultSortOrder = SortOrder.Descending },
					new DataColumn { Key = "configCode", Name = "Тип", Width = 40 },
					new DataColumn { Key = "statusCode", Name = "Статус", Width = 40 /*, Align = DataColumnAlign.Center */ },
					new DataColumn { Key = "name", Name = "Наименование", Sortable = true, Width = 400, UrlProperty = "url" },
					// new DataColumn { Key = "description", Name = "Описание", Width = 300 },
				};
			}

			if (viewId.StartsWith("Classifier/"))
			{
				if (viewId.EndsWith("/okv"))
				{
					result.Fields = new List<FormField>
					{
						new StringField { Key = "code", Name = "Код", Required = true },
						new TextAreaField { Key = "name", Name = "Наименование", Required = true, Rows = 10 },
						new StringField { Key = "digitalCode", Name = "Цифровой код", Required = true },
						new StringField { Key = "shortName", Name = "Краткое наименование" }
					};
				}
				else
				{
					result.Fields = new List<FormField>
					{
						// new StringField { Key = "statusCode", Name = "Статус", Readonly = true },
						new StringField { Key = "code", Name = "Код", Required = true },
						new TextAreaField { Key = "name", Name = "Наименование", Required = true, Rows = 10 }
					};
				}
			}

			if (viewId == "ClassifierGroup/Form")
			{
				result.Fields = new List<FormField>
				{
					// new StringField { Key = "statusCode", Name = "Статус", Readonly = true },
					new StringField { Key = "code", Name = "Код", Required = true },
					new StringField { Key = "name", Name = "Наименование", Required = true },
					new ClassifierField { Key = "parentUid", Name = "Родительская группа" },
				};
			}

			if (viewId == "ClassifierType")
			{
				result.Fields = new List<FormField>
				{
					new StringField { Key = "code", Name = "Код", Required = true },
					new TextAreaField { Key = "name", Name = "Наименование", Rows = 2, Required = true },
					new TextAreaField { Key = "description", Name = "Описание" },
					new SelectField { Key = "hierarchyType", Name = "Иерархия",
						Description = "Справочник может быть без иерархии, с иерархией групп (например, контрагентов можно распределить по группам по их регионам, размеру или отношению к нашей организации) или иерархией элементов (например, одни виды деятельности уточняются другими видами деятельности)",
						Options = new []
						{
							new SelectFieldOption { Value = "None", Name = "Нет" },
							new SelectFieldOption { Value = "Groups", Name = "Группы" },
							new SelectFieldOption { Value = "Items", Name = "Элементы" }
						}},
				};
			}

			if (viewId.StartsWith("ClassifierType/Grid/Hierarchy"))
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "name", Name = "Наименование", Sortable = true, Width = 400 },
					// new DataColumn { Key = "default", Name = "По умолчанию", Width = 10 },
					new DataColumn { Key = "code", Name = "Код", Sortable = true, Width = 10 },
				};
			}
			else if (viewId.StartsWith("ClassifierType/Grid"))
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "name", Name = "Наименование", Sortable = true, Width = 100, UrlProperty = "url" },
					new DataColumn { Key = "description", Name = "Описание", Width = 400 },
					// new DataColumn { Key = "hierarchyType", Name = "Иерархия", Width = 10 }
					new DataColumn { Key = "code", Name = "Код", Sortable = true, Width = 10, UrlProperty = "url" },
				};
			}

			if (viewId.StartsWith("Classifier/Grid"))
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "code", Name = "Код", Sortable = true, Width = 10, UrlProperty = "url" },
					new DataColumn { Key = "name", Name = "Наименование", Sortable = true, Width = 400 },
					new DataColumn { Key = "statusCode", Name = "Статус", Sortable = true, Width = 10 },
				};
			}

			if (viewId == "PrivateEventCounterpartyList/Grid")
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "name", Name = "Организация", Sortable = true, Width = 400 },
					new DataColumn { Key = "email", Name = "E-mail", Sortable = true, Width = 100 },
					// new DataColumn { Key = "description", Name = "Описание", Width = 300 },
				};
			}

			if (viewId == "PrivateEvent/Edit")
			{
				result.Panes = new List<DataPane>
				{
					new DataPane { Key = "tab_info", Name = "Информация", Icon = "profile",
						Component = "panes/private/EditEventPane" },
					new DataPane { Key = "tab_invitations", Name = "Приглашения (0)", Icon = "solution",
						Component = "panes/private/InvitationPane" },
					new DataPane { Key = "tab_proposals", Name = "Предложения", Icon = "solution" },
					new DataPane { Key = "tab_questions", Name = "Разъяснения", Icon = "solution" },
					new DataPane { Key = "tab_team", Name = "Команда", Icon = "team" },
					new DataPane { Key = "tab_items", Name = "Позиции", Icon = "table" },
					new DataPane { Key = "tab_history", Name = "История изменений", Icon = "eye" },
					new DataPane { Key = "tab_5", Name = "Тендерная комиссия (команда?)" },
					new DataPane { Key = "tab_6", Name = "Критерии оценки (анкета?)" },
					new DataPane { Key = "tab_7", Name = "Документы (поле?)" },
					new DataPane { Key = "tab_8", Name = "Контактные лица (поле?)" },
				};
			}

			return await Task.FromResult(result);
		}
	}
}
