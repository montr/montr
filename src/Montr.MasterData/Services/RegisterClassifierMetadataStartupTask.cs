using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.MasterData.Services
{
	// todo: move to impl
	public class RegisterClassifierMetadataStartupTask : IStartupTask
	{
		private readonly IMetadataRegistrator _registrator;

		public RegisterClassifierMetadataStartupTask(IMetadataRegistrator registrator)
		{
			_registrator = registrator;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			_registrator.Register("ClassifierTree/Form", _ => new DataView
			{
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true },
					new TextField { Key = "name", Name = "Наименование", Required = true },
				}
			});

			_registrator.Register("ClassifierGroup/Form", _ => new DataView
			{
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true },
					new TextField { Key = "name", Name = "Наименование", Required = true },
					new ClassifierGroupField { Key = "parentUid", Name = "Родительская группа" },
				}
			});

			_registrator.Register("ClassifierLink/Form", _ => new DataView
			{
				Fields = new List<FieldMetadata>
				{
					new ClassifierGroupField { Key = "group.uid", Name = "Группа", Required = true },
				}
			});

			_registrator.Register("ClassifierType", _ => new DataView
			{
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Props = new TextAreaField.Properties { Rows = 2 } },
					new TextAreaField { Key = "description", Name = "Описание" },
					new SelectField
					{
						Key = "hierarchyType",
						Name = "Иерархия",
						Description = "Классификатор может быть без иерархии, с иерархией групп (например, контрагентов можно распределить по группам по их регионам, размеру или отношению к нашей организации) или иерархией элементов (например, одни виды деятельности уточняются другими видами деятельности)",
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
				}
			});

			_registrator.Register("ClassifierType/Grid/Hierarchy", _ => new DataView
			{
				Columns = new List<DataColumn>
				{
					new() { Key = "name", Name = "Наименование", Sortable = true, Width = 400 },
					// new DataColumn { Key = "default", Name = "По умолчанию", Width = 10 },
					new() { Key = "code", Name = "Код", Sortable = true, Width = 10 },
				}
			});

			_registrator.Register("ClassifierType/Grid/", _ => new DataView
			{
				Columns = new List<DataColumn>
				{
					new() { Key = "name", Name = "Наименование", Sortable = true, Width = 70, UrlProperty = "url" },
					new() { Key = "description", Name = "Описание", Width = 200 },
					// new() { Key = "hierarchyType", Name = "Иерархия", Width = 10 }
					new() { Key = "code", Name = "Код", Sortable = true, Width = 60, UrlProperty = "url" },
				}
			});

			_registrator.Register("Classifier/Grid", _ => new DataView
			{
				Columns = new List<DataColumn>
				{
					new() { Key = "code", Name = "Код", Sortable = true, Width = 10, UrlProperty = "url" },
					new() { Key = "name", Name = "Наименование", Sortable = true, Width = 400, UrlProperty = "url" },
					new() { Key = "statusCode", Name = "Статус", Sortable = true, Width = 30 },
				}
			});

			_registrator.Register("ClassifierLink/Grid", _ => new DataView
			{
				Columns = new List<DataColumn>
				{
					new() { Key = "hierarchy", Name = "Иерархия", Width = 300, Path = "tree.name" },
					new() { Key = "groupCode", Name = "Код группы", Width = 10, Path = "group.code" },
					new() { Key = "groupName", Name = "Группа", Width = 400, Path = "group.name" }
				}
			});

			_registrator.Register("Numerator/Grid", _ => new DataView
			{
				Columns = new List<DataColumn>
				{
					new() { Key = "code", Name = "Код", Sortable = true, UrlProperty = "url", Width = 200 },
					new() { Key = "name", Name = "Наименование", Sortable = true, UrlProperty = "url", Width = 200 },
					new() { Key = "pattern", Name = "Формат номера", UrlProperty = "url", Width = 150 },
					new() { Key = "entityTypeCode", Name = "Применимость", Sortable = true, Width = 30 },
					new() { Key = "periodicity", Name = "Периодичность", Sortable = true, Width = 30 },
					new() { Key = "isActive", Name = "Active", Width = 10, Type = BooleanField.TypeCode },
					new() { Key = "isSystem", Name = "System", Width = 10, Type = BooleanField.TypeCode },
				}
			});

			_registrator.Register("Numerator/Form", _ => new DataView
			{
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Active = true, DisplayOrder = 10, System = true },
					new TextField { Key = "name", Name = "Наименование", Required = true },
					new SelectField
					{
						Key = "entityTypeCode", Name = "EntityTypeCode", Required = true,
						Props =
						{
							Options = new []
							{
								new SelectFieldOption { Value = "DocumentType", Name = "DocumentType" },
								new SelectFieldOption { Value = "ClassifierType", Name = "ClassifierType" },
							}
						}
					},
					new SelectField
					{
						Key = "periodicity", Name = "Periodicity", Required = true,
						Props =
						{
							Options = new []
							{
								new SelectFieldOption { Value = "None", Name = "None" },
								new SelectFieldOption { Value = "Day", Name = "Day" },
								new SelectFieldOption { Value = "Month", Name = "Month" },
								new SelectFieldOption { Value = "Quarter", Name = "Quarter" },
								new SelectFieldOption { Value = "Year", Name = "Year" },
							}
						}
					},
					new TextAreaField { Key = "pattern", Name = "Pattern", Required = true, Props = new TextAreaField.Properties { Rows = 2 } },
				}
			});

			_registrator.Register("NumeratorEntity/Grid", _ => new DataView
			{
				Columns = new List<DataColumn>
				{
					new() { Key = "entityName", Name = "Тип" },
					new() { Key = "isAutoNumbering", Name = "Автонумерация", Type = BooleanField.TypeCode }
				}
			});

			return Task.CompletedTask;
		}
	}
}
