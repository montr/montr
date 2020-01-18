using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	public interface IMetadataProvider
	{
		Task<DataView> GetView(string viewId);
	}

	// todo: move to db?
	public class DefaultMetadataProvider : IMetadataProvider
	{
		private static readonly string PropsPrefix = ExpressionHelper.GetMemberName<TextAreaField>(x => x.Props).ToLowerInvariant();

		private readonly IFieldProviderRegistry _fieldProviderRegistry;

		public DefaultMetadataProvider(IFieldProviderRegistry fieldProviderRegistry)
		{
			_fieldProviderRegistry = fieldProviderRegistry;
		}

		public async Task<DataView> GetView(string viewId)
		{
			var result = new DataView { Id = viewId };

			if (viewId == "Metadata/Grid")
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "displayOrder", Name = "#", Width = 10, Sortable = true },
					new DataColumn { Key = "key", Name = "Key", Width = 100, Sortable = true },
					new DataColumn { Key = "type", Name = "Type", Width = 70, /*Sortable = true*/ },
					new DataColumn { Key = "name", Name = "Name", Width = 150, Sortable = true },
					new DataColumn { Key = "description", Name = "Description", Width = 150 },
					new DataColumn { Key = "active", Name = "Active", Width = 10, Sortable = true, Type = BooleanField.TypeCode },
					new DataColumn { Key = "system", Name = "System", Width = 10, Sortable = true, Type = BooleanField.TypeCode },
					new DataColumn { Key = "required", Name = "Required", Width = 10, Sortable = true, Type = BooleanField.TypeCode },
				};
			}

			if (viewId == "Metadata/Edit")
			{
				var options = _fieldProviderRegistry
					.GetFieldTypes()
					.Where(x => x.IsSystem == false)
					.OrderBy(x => x.Code)
					.Select(x => new SelectFieldOption { Value = x.Code, Name = x.Name })
					.ToArray();

				result.Fields = new List<FieldMetadata>
				{
					// todo: add system type SelectFieldType to select type with icons, common/advanced selection
					new SelectField { Key = "type", Name = "Тип", Required = true, Props = { Options = options } },
					new NumberField { Key = "displayOrder", Name = "#", Required = true, Props = { Min = 0, Max = 256 } },
					new TextField { Key = "key", Name = "Код", Required = true },
					new TextField { Key = "name", Name = "Наименование", Required = true },
					new TextAreaField { Key = "description", Name = "Описание", Props = new TextAreaField.Properties { Rows = 2 } },
					new TextField { Key = "placeholder", Name = "Placeholder" },
					new TextField { Key = "icon", Name = "Icon" },
					// new BooleanField { Key = "readonly", Name = "Readonly" },
					new BooleanField { Key = "required", Name = "Required" }
				};
			}
			else if (viewId.StartsWith("Metadata/Edit/"))
			{
				var code = viewId.Substring("Metadata/Edit/".Length);

				if (code == TextAreaField.TypeCode)
				{
					result.Fields = new List<FieldMetadata>
					{
						new NumberField { Key = PropsPrefix + ".rows", Name = "Количество строк", Props = { Min = 1, Max = byte.MaxValue } }
					};
				}
				else if (code == NumberField.TypeCode)
				{
					result.Fields = new List<FieldMetadata>
					{
						new NumberField { Key = PropsPrefix + ".min", Name = "Минимум", Props = { Min = long.MinValue, Max = long.MaxValue } },
						new NumberField { Key = PropsPrefix + ".max", Name = "Максимум", Props = { Min = long.MinValue, Max = long.MaxValue } }
					};
				}
				else if (code == DecimalField.TypeCode)
				{
					result.Fields = new List<FieldMetadata>
					{
						new NumberField { Key = PropsPrefix + ".min", Name = "Минимум", Props = { Min = decimal.MinValue, Max = decimal.MaxValue } },
						new NumberField { Key = PropsPrefix + ".max", Name = "Максимум", Props = { Min = decimal.MinValue, Max = decimal.MaxValue } },
						new NumberField { Key = PropsPrefix + ".precision", Name = "Точность", Description = "Количество знаков после запятой", Props = { Min = 0, Max = 5 } }
					};
				}
				else if (code == DateField.TypeCode)
				{
					result.Fields = new List<FieldMetadata>
					{
						new BooleanField { Key = PropsPrefix + ".includeTime", Name = "Include Time" }
					};
				}
				else if (code == SelectField.TypeCode)
				{
					result.Fields = new List<FieldMetadata>
					{
						new DesignSelectOptionsField { Key = PropsPrefix + ".options", Required = true, Name = "Options" }
					};
				}
				else if (code == ClassifierField.Code)
				{
					result.Fields = new List<FieldMetadata>
					{
						new SelectClassifierTypeField { Key = PropsPrefix + ".typeCode", Required = true, Name = "Type" }
					};
				}
				else if (code == ClassifierGroupField.Code)
				{
					result.Fields = new List<FieldMetadata>
					{
						new SelectClassifierTypeField { Key = PropsPrefix + ".typeCode", Required = true, Name = "Type" },
						new TextField { Key = PropsPrefix + ".treeCode", Required = true, Name = "Tree" }
					};
				}
			}

			if (viewId == "UserSearch/Grid")
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "userName", Name = "Username", Sortable = true, Width = 400, UrlProperty = "url" },
					new DataColumn { Key = "firstName", Name = "First Name", Sortable = true, Width = 400, UrlProperty = "url" },
					new DataColumn { Key = "lastName", Name = "Last Name", Sortable = true, Width = 400, UrlProperty = "url" },
					new DataColumn { Key = "email", Name = "Email", Sortable = true, Width = 400 },
					new DataColumn { Key = "phoneNumber", Name = "Phone", Sortable = true, Width = 400 },
				};
			}

			if (viewId.StartsWith("Classifier/"))
			{
				if (viewId.EndsWith("/okv"))
				{
					result.Fields = new List<FieldMetadata>
					{
						new TextField { Key = "code", Name = "Код", Required = true },
						new TextAreaField { Key = "name", Name = "Наименование", Required = true, Props = new TextAreaField.Properties { Rows = 10 } },
						new TextField { Key = "digitalCode", Name = "Цифровой код", Required = true },
						new TextField { Key = "shortName", Name = "Краткое наименование" }
					};

					// todo: remove, only to test long form
					for (var i = 0; i < 100; i++)
					{
						result.Fields.Add(new TextField { Key = "test" + i, Name = "Тестовое поле №" + i});
					}
				}
				else
				{
					result.Fields = new List<FieldMetadata>
					{
						// new TextField { Key = "statusCode", Name = "Статус", Readonly = true },
						new TextField { Key = "code", Name = "Код", Required = true },
						new TextAreaField { Key = "name", Name = "Наименование", Required = true, Props = new TextAreaField.Properties { Rows = 10 } }
					};
				}
			}

			if (viewId == "ClassifierTree/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true },
					new TextField { Key = "name", Name = "Наименование", Required = true },
				};
			}

			if (viewId == "ClassifierGroup/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true },
					new TextField { Key = "name", Name = "Наименование", Required = true },
					new ClassifierGroupField { Key = "parentUid", Name = "Родительская группа" },
				};
			}

			if (viewId == "ClassifierLink/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new ClassifierGroupField { Key = "group.uid", Name = "Группа", Required = true },
				};
			}

			if (viewId == "ClassifierType")
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Props = new TextAreaField.Properties { Rows = 2 } },
					new TextAreaField { Key = "description", Name = "Описание" },
					new SelectField
					{
						Key = "hierarchyType", Name = "Иерархия",
						Description = "Справочник может быть без иерархии, с иерархией групп (например, контрагентов можно распределить по группам по их регионам, размеру или отношению к нашей организации) или иерархией элементов (например, одни виды деятельности уточняются другими видами деятельности)",
						Props =
						{
							Options = new []
							{
								new SelectFieldOption { Value = "None", Name = "Нет" },
								new SelectFieldOption { Value = "Groups", Name = "Группы" },
								new SelectFieldOption { Value = "Items", Name = "Элементы" }
							}
						}
					}
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
					new DataColumn { Key = "description", Name = "Описание", Width = 200 },
					// new DataColumn { Key = "hierarchyType", Name = "Иерархия", Width = 10 }
					new DataColumn { Key = "code", Name = "Код", Sortable = true, Width = 60, UrlProperty = "url" },
				};
			}

			if (viewId.StartsWith("Classifier/Grid"))
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "code", Name = "Код", Sortable = true, Width = 10, UrlProperty = "url" },
					new DataColumn { Key = "name", Name = "Наименование", Sortable = true, Width = 400 },
					new DataColumn { Key = "statusCode", Name = "Статус", Sortable = true, Width = 30 },
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
					new DataColumn { Key = "value", Name = "Значение", Width = 200 }
				};
			}

			// Montr.Idx
			if (viewId == "Login/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "email", Name = "Email", Placeholder = "Your email", Icon = "user", Required = true },
					new PasswordField { Key = "password", Name = "Password", Placeholder = "Password", Icon = "lock", Required = true },
					new BooleanField { Key = "rememberMe", Name = "Remember me?" }
				};
			}

			if (viewId == "ForgotPassword/Form" || viewId == "SendEmailConfirmation/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "email", Name = "Email", Icon = "mail", Required = true }
				};
			}

			if (viewId == "ResetPassword/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "email", Name = "Email", Required = true },
					new PasswordField { Key = "password", Name = "Password", Required = true },
					new PasswordField { Key = "confirmPassword", Name = "Confirm Password", Required = true }
				};
			}

			if (viewId == "ChangePassword/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new PasswordField { Key = "oldPassword", Name = "Current password", Required = true },
					new PasswordField { Key = "newPassword", Name = "New password", Required = true },
					new PasswordField { Key = "confirmPassword", Name = "Confirm new password", Required = true }
				};
			}

			if (viewId == "SetPassword/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new PasswordField { Key = "newPassword", Name = "New password", Required = true },
					new PasswordField { Key = "confirmPassword", Name = "Confirm new password", Required = true }
				};
			}

			if (viewId == "Register/Form")
			{
				// todo: add terms and conditions checkbox
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "email", Name = "Email", Required = true },
					new TextField { Key = "firstName", Name = "First Name", Required = true },
					new TextField { Key = "lastName", Name = "Last Name", Required = true },
					new PasswordField { Key = "password", Name = "Password", Required = true },
					// new PasswordField { Key = "confirmPassword", Name = "Confirm Password", Required = true }
				};
			}

			if (viewId == "ExternalRegister/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "email", Name = "Email", Required = true },
					new TextField { Key = "firstName", Name = "First Name", Required = true },
					new TextField { Key = "lastName", Name = "Last Name", Required = true }
				};
			}

			if (viewId == "ChangeEmail/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "email", Name = "Email", Required = true }
				};
			}

			if (viewId == "ChangePhone/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "phoneNumber", Name = "Номер телефона" } // todo: PhoneField
				};
			}

			if (viewId == "UpdateProfile/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "userName", Name = "Имя пользователя", Readonly = true },
					new TextField { Key = "firstName", Name = "Имя" },
					new TextField { Key = "lastName", Name = "Фамилия" }
				};
			}

			// Documents & Processes
			if (viewId == "Process/List")
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "code", Name = "Code", Sortable = true, UrlProperty = "url", Width = 100 },
					new DataColumn { Key = "name", Name = "Name", Sortable = true, UrlProperty = "url", Width = 400 }
				};
			}

			// Events
			if (viewId == "PrivateEventSearch/Grid")
			{
				result.Columns = new List<DataColumn>
				{
					new DataColumn { Key = "id", Name = "Номер", Sortable = true, Width = 10,
						UrlProperty = "url", DefaultSortOrder = SortOrder.Descending },
					new DataColumn { Key = "configCode", Name = "Тип", Width = 25 },
					new DataColumn { Key = "statusCode", Name = "Статус", Width = 25 /*, Align = DataColumnAlign.Center */ },
					new DataColumn { Key = "name", Name = "Наименование", Sortable = true, Width = 400, UrlProperty = "url" },
					// new DataColumn { Key = "description", Name = "Описание", Width = 300 },
				};
			}

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
				result.Fields = new List<FieldMetadata>
				{
					new ClassifierField { Key = "counterpartyUid", Name = "Контрагент", Required = true, Props = { TypeCode = "counterparty" } },
					new TextField { Key = "user", Name = "Пользователь" },
					new TextField { Key = "email", Name = "Email", Required = true },
					new TextField { Key = "phone", Name = "Телефон" },
				};
			}

			return await Task.FromResult(result);
		}
	}
}
