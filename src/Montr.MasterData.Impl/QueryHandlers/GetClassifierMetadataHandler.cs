using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
		private readonly IConfigurationManager _configurationManager;
		private readonly IRepository<FieldMetadata> _metadataRepository;

		public GetClassifierMetadataHandler(
			IClassifierTypeService classifierTypeService,
			IConfigurationManager configurationManager,
			IRepository<FieldMetadata> metadataRepository)
		{
			_classifierTypeService = classifierTypeService;
			_configurationManager = configurationManager;
			_metadataRepository = metadataRepository;
		}

		public async Task<DataView> Handle(GetClassifierMetadata request, CancellationToken cancellationToken)
		{
			if (request.ViewId == ViewCode.ClassifierTypeTabs)
			{
				var entity = request.TypeCode != null
					? await GetClassifierType(request, cancellationToken)
					: new ClassifierType(); // for new classifier types

				// todo: authorize before sort
				var items = _configurationManager.GetItems<ClassifierType, DataPane>(entity);

				return new DataView
				{
					Panes = items.OrderBy(x => x.DisplayOrder).ToImmutableList()
				};
			}

			if (request.ViewId == ViewCode.NumeratorEntityForm)
			{
				return GetNumeratorEntityForm();
			}

			var classifierType = await GetClassifierType(request, cancellationToken);

			if (request.ViewId == ViewCode.ClassifierTabs)
			{
				return GetClassifierTabs(classifierType);
			}

			return await GetClassifierForm(classifierType, cancellationToken);
		}

		private async Task<ClassifierType> GetClassifierType(GetClassifierMetadata request, CancellationToken cancellationToken)
		{
			var typeCode = request.TypeCode ?? throw new ArgumentNullException(nameof(request.TypeCode));

			return await _classifierTypeService.Get(typeCode, cancellationToken);
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

			if (type.Code == ClassifierTypeCode.Numerator)
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
