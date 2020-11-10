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
			return new DataView
			{
				Panes = new List<DataPane>
				{
					new DataPane { Key = "info", Name = "Информация", Component = "panes/TabEditClassifierType" },
					new DataPane { Key = "hierarchy", Name = "Иерархия", Component = "panes/TabEditClassifierTypeHierarchy" },
					new DataPane { Key = "fields", Name = "Поля", Component = "panes/PaneSearchMetadata" },
					new DataPane { Key = "history", Name = "История изменений" }
				}
			};
		}

		private static DataView GetClassifierTabs(ClassifierType type)
		{
			var panes = new List<DataPane>
			{
				new DataPane { Key = "info", Name = "Информация", Component = "panes/TabEditClassifier" },
				new DataPane { Key = "hierarchy", Name = "Иерархия", Component = "panes/TabEditClassifierHierarchy" },
				new DataPane { Key = "dependencies", Name = "Зависимости" },
				new DataPane { Key = "history", Name = "История изменений" }
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
					new DataPane { Key = "usage", Name = "Использование", Component = "panes/PaneNumeratorEntityList" });
			}

			// todo: move to own module
			if (type.Code == "process")
			{
				panes.Insert(panes.FindIndex(x => x.Key == "info") + 1,
					new DataPane { Key = "steps", Name = "Шаги", Component = "panes/PaneProcessStepList" });
			}

			return new DataView { Panes = panes };
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
