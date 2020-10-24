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
			var typeCode = request.TypeCode ?? throw new ArgumentNullException(nameof(request.TypeCode));

			var type = await _classifierTypeService.Get(typeCode, cancellationToken);

			if (request.ViewId == "Classifier/Tabs")
			{
				return GetTabsMetadata(type);
			}

			return await GetFormMetadata(type, cancellationToken);
		}

		private static DataView GetTabsMetadata(ClassifierType type)
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
					new DataPane { Key = "questionnaire", Name = "Вопросы" });
			}

			return new DataView { Panes = panes };
		}

		private async Task<DataView> GetFormMetadata(ClassifierType type, CancellationToken cancellationToken)
		{
			var metadata = await _metadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = ClassifierType.EntityTypeCode,
				EntityUid = type.Uid,
				IsActive = true
			}, cancellationToken);

			var dbFields = new List<string>
			{
				nameof(Classifier.Code),
				nameof(Classifier.Name)
			};

			var result = new DataView { Fields = metadata.Rows };

			foreach (var field in result.Fields)
			{
				// if (field.System == false)
				if (dbFields.Contains(field.Key, StringComparer.InvariantCultureIgnoreCase) == false)
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
