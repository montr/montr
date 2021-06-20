using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;
using Montr.Metadata.Models;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierMetadataHandler : IRequestHandler<GetClassifierMetadata, DataView>
	{
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IRepository<FieldMetadata> _metadataRepository;

		public GetClassifierMetadataHandler(
			IClassifierTypeService classifierTypeService,
			IRepository<FieldMetadata> metadataRepository)
		{
			_classifierTypeService = classifierTypeService;
			_metadataRepository = metadataRepository;
		}

		public async Task<DataView> Handle(GetClassifierMetadata request, CancellationToken cancellationToken)
		{
			if (request.ViewId == "ClassifierType/Tabs")
			{
				return GetClassifierTypeTabs();
			}

			if (request.ViewId == "NumeratorEntity/Form")
			{
				return GetNumeratorEntityForm();
			}

			var typeCode = request.TypeCode ?? throw new ArgumentNullException(nameof(request.TypeCode));

			var type = await _classifierTypeService.Get(typeCode, cancellationToken);

			if (request.ViewId == "Classifier/Tabs")
			{
				return GetClassifierTabs(type);
			}

			return await GetClassifierForm(type, cancellationToken);
		}

		private static DataView GetClassifierTypeTabs()
		{
			return new()
			{
				Panes = new List<DataPane>
				{
					new() { Key = "info", Name = "Информация", Icon = "profile", Component = "panes/TabEditClassifierType" },
					new() { Key = "hierarchy", Name = "Иерархия", Component = "panes/TabEditClassifierTypeHierarchy" },
					new() { Key = "fields", Name = "Поля", Component = "panes/PaneSearchMetadata" },
					new() { Key = "numeration", Name = "Нумерация", Component = "panes/PaneEditNumeration" },
					new() { Key = "history", Name = "История изменений", Icon = "eye" }
				}
			};
		}

		private static DataView GetClassifierTabs(ClassifierType type)
		{
			var panes = new List<DataPane>
			{
				new() { Key = "info", Name = "Информация", Icon = "profile", Component = "panes/TabEditClassifier" },
				new() { Key = "hierarchy", Name = "Иерархия", Component = "panes/TabEditClassifierHierarchy" },
				new() { Key = "dependencies", Name = "Зависимости" },
				new() { Key = "history", Name = "История изменений", Icon = "eye" }
			};

			// todo: register different tabs for different classifiers on startup
			if (type.Code == "questionnaire")
			{
				panes.Insert(panes.FindIndex(x => x.Key == "info") + 1,
					new DataPane { Key = "questionnaire", Name = "Вопросы", Component = "panes/PaneSearchMetadata" });
			}

			if (type.Code == "numerator")
			{
				panes.Insert(panes.FindIndex(x => x.Key == "info") + 1,
					new DataPane { Key = "usage", Name = "Использование", Component = "panes/TabEditNumeratorEntities" });
			}

			if (type.Code == "role")
			{
				panes.Insert(panes.FindIndex(x => x.Key == "info") + 1,
					new DataPane { Key = "permissions", Name = "Permissions", Component = "components/tab-edit-role-permissions" });
			}

			if (type.Code == "user")
			{
				panes.Insert(panes.FindIndex(x => x.Key == "info") + 1,
					new DataPane { Key = "roles", Name = "Roles", Icon = "solution", Component = "components/tab-edit-user-roles" });
			}

			// todo: move to own module
			if (type.Code == "process")
			{
				panes.Insert(panes.FindIndex(x => x.Key == "info") + 1,
					new DataPane { Key = "steps", Name = "Шаги", Component = "panes/PaneProcessStepList" });
			}

			if (type.Code == "document_type")
			{
				var index = panes.FindIndex(x => x.Key == "info");

				panes.Insert(++index, new DataPane { Key = "fields", Name = "Анкета", Component = "panes/PaneSearchMetadata" });

				// todo: move to processes (?)
				panes.Insert(++index, new DataPane { Key = "statuses", Name = "Statuses", Component = "panes/PaneSearchEntityStatuses" });
				panes.Insert(++index, new DataPane { Key = "automation", Name = "Automations", Component = "panes/PaneSearchAutomation" });
			}

			return new DataView { Panes = panes };
		}

		private DataView GetNumeratorEntityForm()
		{
			return new()
			{
				Fields = new List<FieldMetadata>
				{
					new BooleanField
					{
						Key = "isAutoNumbering",
						Name = "Автонумерация",
						Required = true
					},
					new ClassifierField
					{
						Key = "numeratorUid",
						Name = "Нумератор",
						Props = { TypeCode = ClassifierTypeCode.Numerator }
					}
				}
			};
		}

		private async Task<DataView> GetClassifierForm(ClassifierType type, CancellationToken cancellationToken)
		{
			var metadata = await _metadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = ClassifierType.TypeCode,
				EntityUid = type.Uid,
				IsActive = true
			}, cancellationToken);

			var result = new DataView { Fields = metadata.Rows };

			foreach (var field in result.Fields)
			{
				if (field.System == false)
				{
					field.Key = FieldKey.FormatFullKey(field.Key);
				}
			}

			var commonFields = GetCommonFields(type);
			if (commonFields != null)
			{
				result.Fields = commonFields.Union(result.Fields).ToArray();
			}

			return result;
		}

		// todo: parent group or item should be edited outside classifier fields
		private static ICollection<FieldMetadata> GetCommonFields(ClassifierType type)
		{
			if (type.HierarchyType == HierarchyType.Groups)
			{
				return new List<FieldMetadata>
				{
					new ClassifierGroupField
					{
						Key = "parentUid",
						Name = "Группа",
						Required = true,
						Props = { TypeCode = type.Code, TreeCode = ClassifierTree.DefaultCode }
					}
				};
			}

			if (type.HierarchyType == HierarchyType.Items)
			{
				return new List<FieldMetadata>
				{
					new ClassifierGroupField
					{
						Key = "parentUid",
						Name = "Родительский элемент",
						Props = { TypeCode = type.Code }
					},
				};
			}

			return null;
		}
	}
}
