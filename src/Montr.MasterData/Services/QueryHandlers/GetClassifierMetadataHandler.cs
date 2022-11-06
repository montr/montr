using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.Metadata.Models;

namespace Montr.MasterData.Services.QueryHandlers
{
	public class GetClassifierMetadataHandler : IRequestHandler<GetClassifierMetadata, DataView>
	{
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IConfigurationService _configurationService;
		private readonly IRepository<FieldMetadata> _metadataRepository;

		public GetClassifierMetadataHandler(
			IClassifierTypeService classifierTypeService,
			IConfigurationService configurationService,
			IRepository<FieldMetadata> metadataRepository)
		{
			_classifierTypeService = classifierTypeService;
			_configurationService = configurationService;
			_metadataRepository = metadataRepository;
		}

		public async Task<DataView> Handle(GetClassifierMetadata request, CancellationToken cancellationToken)
		{
			if (request.ViewId == ViewCode.NumeratorEntityForm)
			{
				return GetNumeratorEntityForm();
			}

			var typeCode = request.TypeCode ?? throw new ArgumentNullException(nameof(request.TypeCode));

			var classifierType = await _classifierTypeService.Get(typeCode, cancellationToken);

			if (request.ViewId == ViewCode.ClassifierTabs)
			{
				// todo: preload item from service
				var classifier = new Classifier { Type = classifierType.Code };

				return new DataView
				{
					Panes = await _configurationService.GetItems<Classifier, DataPane>(classifier, request.Principal)
				};
			}

			return await GetClassifierForm(classifierType, cancellationToken);
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
			// todo: move to IClassifierTypeMetadataService,
			var metadata = await _metadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = ClassifierType.TypeCode,
				EntityUid = type.Uid.Value,
				IsActive = true
			}, cancellationToken);

			var result = new DataView { Fields = metadata.Rows };

			// todo: check system fields as properties of DbClassifier
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
