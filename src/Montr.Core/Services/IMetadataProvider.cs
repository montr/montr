﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IMetadataProvider
	{
		Task<DataView> GetView(string viewId);
	}

	// todo: move to db?
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

			if (viewId == "ClassifierTree/Form")
			{
				result.Fields = new List<FormField>
				{
					new StringField { Key = "code", Name = "Код", Required = true },
					new StringField { Key = "name", Name = "Наименование", Required = true },
				};
			}

			if (viewId == "ClassifierGroup/Form")
			{
				result.Fields = new List<FormField>
				{
					new StringField { Key = "code", Name = "Код", Required = true },
					new StringField { Key = "name", Name = "Наименование", Required = true },
					new ClassifierGroupField { Key = "parentUid", Name = "Родительская группа" },
				};
			}

			if (viewId == "ClassifierLink/Form")
			{
				result.Fields = new List<FormField>
				{
					new ClassifierGroupField { Key = "group.uid", Name = "Группа", Required = true },
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

			if (viewId.StartsWith("ClassifierLink/Grid"))
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "hierarchy", Name = "Иерархия", Width = 300, Path = "tree.name" },
					new DataColumn { Key = "groupCode", Name = "Код группы", Width = 10, Path = "group.code" },
					new DataColumn { Key = "groupName", Name = "Группа", Width = 400, Path = "group.name" }
				};
			}

			// Core
			if (viewId.StartsWith("LocaleString/Grid"))
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "locale", Name = "Язык", Width = 20, Sortable = true },
					new DataColumn { Key = "module", Name = "Модуль", Width = 60, Sortable = true },
					new DataColumn { Key = "key", Name = "Ключ", Width = 100, Sortable = true },
					new DataColumn { Key = "value", Name = "Значение", Width = 400 }
				};
			}

			// Montr.Idx
			if (viewId == "Login/Form")
			{
				result.Fields = new List<FormField>
				{
					new StringField { Key = "email", Name = "Email", Required = true },
					new PasswordField { Key = "password", Name = "Password", Required = true },
					new BooleanField { Key = "rememberMe", Name = "Remember me?" }
				};
			}

			if (viewId == "ForgotPassword/Form" || viewId == "SendEmailConfirmation/Form")
			{
				result.Fields = new List<FormField>
				{
					new StringField { Key = "email", Name = "Email", Required = true }
				};
			}
			
			if (viewId == "ResetPassword/Form")
			{
				result.Fields = new List<FormField>
				{
					new StringField { Key = "email", Name = "Email", Required = true },
					new PasswordField { Key = "password", Name = "Password", Required = true }
					// new PasswordField { Key = "confirmPassword", Name = "Confirm Password", Required = true }
				};
			}

			if (viewId == "Register/Form")
			{
				result.Fields = new List<FormField>
				{
					new StringField { Key = "email", Name = "Email", Required = true },
					new StringField { Key = "firstName", Name = "First Name", Required = true },
					new StringField { Key = "lastName", Name = "Last Name", Required = true },
					new PasswordField { Key = "password", Name = "Password", Required = true },
					// new PasswordField { Key = "confirmPassword", Name = "Confirm Password", Required = true }
				};
			}

			if (viewId == "ExternalRegister/Form")
			{
				result.Fields = new List<FormField>
				{
					new StringField { Key = "email", Name = "Email", Required = true },
					new StringField { Key = "firstName", Name = "First Name", Required = true },
					new StringField { Key = "lastName", Name = "Last Name", Required = true }
				};
			}

			// Events
			if (viewId == "PrivateEvent/Edit")
			{
				result.Panes = new List<DataPane>
				{
					new DataPane { Key = "info", Name = "Информация", Icon = "profile", Component = "panes/private/EditEventPane" },
					new DataPane { Key = "invitations", Name = "Приглашения (0)", Icon = "solution", Component = "panes/private/InvitationPane" },
					new DataPane { Key = "proposals", Name = "Предложения", Icon = "solution" },
					new DataPane { Key = "questions", Name = "Разъяснения", Icon = "solution" },
					new DataPane { Key = "team", Name = "Команда", Icon = "team" },
					new DataPane { Key = "items", Name = "Позиции", Icon = "table" },
					new DataPane { Key = "history", Name = "История изменений", Icon = "eye" },
					new DataPane { Key = "5", Name = "Тендерная комиссия (команда?)" },
					new DataPane { Key = "6", Name = "Критерии оценки (анкета?)" },
					new DataPane { Key = "7", Name = "Документы (поле?)" },
					new DataPane { Key = "8", Name = "Контактные лица (поле?)" },
				};
			}

			if (viewId == "Event/Invitation/List")
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "counterpartyName", Name = "Контрагент", Sortable = true, Width = 400 },
					new DataColumn { Key = "statusCode", Name = "Статус", Width = 100 },
					new DataColumn { Key = "user", Name = "Контактное лицо", Width = 100 },
					new DataColumn { Key = "email", Name = "Email", Width = 100 },
					new DataColumn { Key = "phone", Name = "Телефон", Width = 100 },
					new DataColumn { Key = "createDate", Name = "Дата создания", Width = 100 },
					new DataColumn { Key = "inviteDate", Name = "Дата приглашения", Width = 100 },
					new DataColumn { Key = "lastAccessDate", Name = "Дата последнего доступа", Width = 100 },
				};
			}

			if (viewId == "Event/Invitation/Form")
			{
				result.Fields = new List<FormField>
				{
					new ClassifierField { Key = "counterpartyUid", Name = "Контрагент", TypeCode = "counterparty", Required = true },
					new StringField { Key = "user", Name = "Пользователь" },
					new StringField { Key = "email", Name = "Email", Required = true },
					new StringField { Key = "phone", Name = "Телефон" },
				};
			}

			return await Task.FromResult(result);
		}
	}
}
