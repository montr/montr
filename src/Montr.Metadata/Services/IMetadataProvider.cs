using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Montr.Core.Models;
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
		private readonly IFieldProviderRegistry _fieldProviderRegistry;
		private readonly IMetadataRegistrator _registrator;

		public DefaultMetadataProvider(IFieldProviderRegistry fieldProviderRegistry, IMetadataRegistrator registrator)
		{
			_fieldProviderRegistry = fieldProviderRegistry;
			_registrator = registrator;
		}

		public async Task<DataView> GetView(string viewId)
		{
			if (_registrator.TryGet(viewId, out var dataView))
			{
				return dataView;
			}

			var result = new DataView { Id = viewId };

			if (viewId == "Setup/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "companyName", Name = "Наименование компании", Required = true, Description = "Наименовании компании-оператора системы" },
					new TextField { Key = "adminEmail", Name = "Email администратора", Required = true },
					new PasswordField { Key = "adminPassword", Name = "Пароль администратора", Required = true },
				};
			}

			if (viewId == "EntityStatus/Grid")
			{
				result.Columns = new List<DataColumn>
				{
					new() { Key = "displayOrder", Name = "#", Width = 10, Sortable = true },
					new() { Key = "code", Name = "Code", Width = 70, Sortable = true },
					new() { Key = "name", Name = "Name", Width = 550, Sortable = true }
				};
			}

			if (viewId == "EntityStatus/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new NumberField { Key = "displayOrder", Name = "Номер", Required = true, Props = { Min = 0, Max = 256 } },
					new TextField { Key = "code", Name = "Код", Required = true },
					new TextField { Key = "name", Name = "Наименование", Required = true }
				};
			}

			if (viewId == "Metadata/Grid")
			{
				result.Columns = new List<DataColumn>
				{
					new() { Key = "displayOrder", Name = "#", Width = 10, Sortable = true },
					new() { Key = "key", Name = "Key", Width = 100, Sortable = true },
					new() { Key = "type", Name = "Type", Width = 70, /*Sortable = true*/ },
					new() { Key = "name", Name = "Name", Width = 150, Sortable = true },
					new() { Key = "description", Name = "Description", Width = 150 },
					new() { Key = "active", Name = "Active", Width = 10, Sortable = true, Type = BooleanField.TypeCode },
					new() { Key = "system", Name = "System", Width = 10, Sortable = true, Type = BooleanField.TypeCode },
					new() { Key = "required", Name = "Required", Width = 10, Sortable = true, Type = BooleanField.TypeCode },
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
				};
			}

			if (viewId.StartsWith("Metadata/Edit/"))
			{
				var code = viewId.Substring("Metadata/Edit/".Length);

				var fieldProvider = _fieldProviderRegistry.GetFieldTypeProvider(code);

				result.Fields = fieldProvider.GetMetadata();
			}

			if (viewId == "UserRoles/Grid")
			{
				result.Columns = new List<DataColumn>
				{
					new() { Key = "name", Name = "Name", Sortable = true, Width = 1000, UrlProperty = "url" }
				};
			}

			if (viewId == "RolePermissions/Grid")
			{
				result.Columns = new List<DataColumn>
				{
					new() { Key = "code", Name = "Code", Width = 1000, UrlProperty = "url" }
				};
			}

			if (viewId == "CompanySearch/Grid")
			{
				result.Columns = new List<DataColumn>
				{
					new() { Key = "name", Name = "Наименование", Sortable = true, Width = 200, UrlProperty = "url" },
					new() { Key = "configCode", Name = "ConfigCode", Sortable = true, Width = 100 },
					new() { Key = "statusCode", Name = "StatusCode", Sortable = true, Width = 100 }
				};
			}

			// Core
			if (viewId.StartsWith("LocaleString/Grid"))
			{
				result.Columns = new List<DataColumn>
				{
					new() { Key = "locale", Name = "Язык", Width = 20, Sortable = true },
					new() { Key = "module", Name = "Модуль", Width = 60, Sortable = true },
					new() { Key = "key", Name = "Ключ", Width = 100, Sortable = true },
					new() { Key = "value", Name = "Значение", Width = 200 }
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

			if (viewId == "Document/List")
			{
				result.Columns = new List<DataColumn>
				{
					new() { Key = "documentNumber", Name = "Номер", Sortable = true, UrlProperty = "url", Width = 50 },
					new() { Key = "documentDate", Name = "Дата", Type = "datetime", Sortable = true, UrlProperty = "url", Width = 100 },
					new() { Key = "direction", Name = "Направление", UrlProperty = "url", Width = 30 },
					new() { Key = "name", Name = "Наименование", Width = 250 },
					new() { Key = "configCode", Name = "Тип", Sortable = true, Width = 100 },
					new() { Key = "statusCode", Name = "Статус", Sortable = true, Width = 100 },
				};
			}

			// Events
			if (viewId == "PrivateEventSearch/Grid")
			{
				result.Columns = new List<DataColumn>
				{
					new()
					{ Key = "id", Name = "Номер", Sortable = true, Width = 10,
						UrlProperty = "url", DefaultSortOrder = SortOrder.Descending },
					new() { Key = "configCode", Name = "Тип", Width = 25 },
					new() { Key = "statusCode", Name = "Статус", Width = 25 /*, Align = DataColumnAlign.Center */ },
					new() { Key = "name", Name = "Наименование", Sortable = true, Width = 400, UrlProperty = "url" },
					// new DataColumn { Key = "description", Name = "Описание", Width = 300 },
				};
			}

			if (viewId == "Event/Invitation/List")
			{
				result.Columns = new List<DataColumn>
				{
					new() { Key = "counterpartyName", Name = "Контрагент", Sortable = true, Width = 400 },
					new() { Key = "statusCode", Name = "Статус", Width = 100 },
					new() { Key = "user", Name = "Контактное лицо", Width = 100 },
					new() { Key = "email", Name = "Email", Width = 100 },
					new() { Key = "phone", Name = "Телефон", Width = 100 },
					new() { Key = "createDate", Name = "Дата создания", Width = 100 },
					new() { Key = "inviteDate", Name = "Дата приглашения", Width = 100 },
					new() { Key = "lastAccessDate", Name = "Дата последнего доступа", Width = 100 },
				};
			}

			/*if (viewId == "Event/Invitation/Form")
			{
				result.Fields = new List<FieldMetadata>
				{
					new ClassifierField { Key = "counterpartyUid", Name = "Контрагент", Required = true, Props = { TypeCode = "counterparty" } },
					new TextField { Key = "user", Name = "Пользователь" },
					new TextField { Key = "email", Name = "Email", Required = true },
					new TextField { Key = "phone", Name = "Телефон" },
				};
			}*/

			return await Task.FromResult(result);
		}
	}
}
