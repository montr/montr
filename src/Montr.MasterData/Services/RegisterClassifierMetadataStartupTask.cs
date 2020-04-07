using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.MasterData.Services
{
	public class RegisterClassifierMetadataStartupTask : IStartupTask
	{
		private readonly IMetadataRegistrator _registrator;

		public RegisterClassifierMetadataStartupTask(IMetadataRegistrator registrator)
		{
			_registrator = registrator;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			/*if (viewId.StartsWith("Classifier/"))
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
						result.Fields.Add(new TextField { Key = "test" + i, Name = "Тестовое поле №" + i });
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
			}*/

			_registrator.Register("ClassifierTree/Form", viewId => new DataView
			{
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true },
					new TextField { Key = "name", Name = "Наименование", Required = true },
				}
			});

			_registrator.Register("ClassifierGroup/Form", viewId => new DataView
			{
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true },
					new TextField { Key = "name", Name = "Наименование", Required = true },
					new ClassifierGroupField { Key = "parentUid", Name = "Родительская группа" },
				}
			});

			_registrator.Register("ClassifierLink/Form", viewId => new DataView
			{
				Fields = new List<FieldMetadata>
				{
					new ClassifierGroupField { Key = "group.uid", Name = "Группа", Required = true },
				}
			});

			_registrator.Register("ClassifierType", viewId => new DataView
			{
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Props = new TextAreaField.Properties { Rows = 2 } },
					new TextAreaField { Key = "description", Name = "Описание" },
					new SelectField
					{
						Key = "hierarchyType", Name = "Иерархия",
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

			_registrator.Register("ClassifierType/Grid/Hierarchy", viewId => new DataView
			{
				Columns = new List<DataColumn>
				{
					new DataColumn { Key = "name", Name = "Наименование", Sortable = true, Width = 400 },
					// new DataColumn { Key = "default", Name = "По умолчанию", Width = 10 },
					new DataColumn { Key = "code", Name = "Код", Sortable = true, Width = 10 },
				}
			});

			_registrator.Register("ClassifierType/Grid/", viewId => new DataView
			{
				Columns = new List<DataColumn>
				{
					new DataColumn { Key = "name", Name = "Наименование", Sortable = true, Width = 100, UrlProperty = "url" },
					new DataColumn { Key = "description", Name = "Описание", Width = 200 },
					// new DataColumn { Key = "hierarchyType", Name = "Иерархия", Width = 10 }
					new DataColumn { Key = "code", Name = "Код", Sortable = true, Width = 60, UrlProperty = "url" },
				}
			});

			_registrator.Register("Classifier/Grid", viewId => new DataView
			{
				Columns = new List<DataColumn>
				{
					new DataColumn { Key = "code", Name = "Код", Sortable = true, Width = 10, UrlProperty = "url" },
					new DataColumn { Key = "name", Name = "Наименование", Sortable = true, Width = 400 },
					new DataColumn { Key = "statusCode", Name = "Статус", Sortable = true, Width = 30 },
				}
			});

			_registrator.Register("ClassifierLink/Grid", viewId => new DataView
			{
				Columns = new List<DataColumn>
				{
					new DataColumn { Key = "hierarchy", Name = "Иерархия", Width = 300, Path = "tree.name" },
					new DataColumn { Key = "groupCode", Name = "Код группы", Width = 10, Path = "group.code" },
					new DataColumn { Key = "groupName", Name = "Группа", Width = 400, Path = "group.name" }
				}
			});

			_registrator.Register("Numerator/Grid", viewId => new DataView
			{
				Columns = new List<DataColumn>
				{
					new DataColumn { Key = "name", Name = "Наименование", Sortable = true, UrlProperty = "url", Width = 200 },
					new DataColumn { Key = "pattern", Name = "Формат номера", UrlProperty = "url", Width = 150 },
					new DataColumn { Key = "entityTypeCode", Name = "Применимость", Sortable = true, Width = 30 },
					new DataColumn { Key = "periodicity", Name = "Периодичность", Sortable = true, Width = 30 },
					new DataColumn { Key = "isActive", Name = "Active", Width = 10, Type = BooleanField.TypeCode },
					new DataColumn { Key = "isSystem", Name = "System", Width = 10, Type = BooleanField.TypeCode },
				}
			});

			_registrator.Register("Numerator/Form", viewId => new DataView
			{
				Fields = new List<FieldMetadata>
				{
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
					new TextField { Key = "name", Name = "Наименование", Required = true },
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
					new TextAreaField { Key = "pattern", Name = "Pattern", Required = true, Props = new TextAreaField.Properties { Rows = 4 } },
				}
			});

			return Task.CompletedTask;
		}
	}
}
