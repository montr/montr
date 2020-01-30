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
using Montr.Metadata.Services;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierMetadataHandler : IRequestHandler<GetClassifierMetadata, DataView>
	{
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IMetadataProvider _metadataProvider;
		private readonly IRepository<FieldMetadata> _repository;

		public GetClassifierMetadataHandler(IClassifierTypeService classifierTypeService,
			IMetadataProvider metadataProvider, IRepository<FieldMetadata> repository)
		{
			_classifierTypeService = classifierTypeService;
			_metadataProvider = metadataProvider;
			_repository = repository;
		}

		public async Task<DataView> Handle(GetClassifierMetadata request, CancellationToken cancellationToken)
		{
			var typeCode = request.TypeCode ?? throw new ArgumentNullException(nameof(request.TypeCode));

			var type = await _classifierTypeService.GetClassifierType(typeCode, cancellationToken);

			ICollection<FieldMetadata> commonFields = null;

			if (type.HierarchyType == HierarchyType.Groups)
			{
				commonFields = new List<FieldMetadata>
				{
					new ClassifierGroupField
					{
						Key = "parentUid", Name = "Группа", Required = true,
						Props = { TypeCode = typeCode, TreeCode = ClassifierTree.DefaultCode }
					}
				};
			}
			else if (type.HierarchyType == HierarchyType.Items)
			{
				commonFields = new List<FieldMetadata>
				{
					new ClassifierGroupField
					{
						Key = "parentUid", Name = "Родительский элемент", Props = { TypeCode = typeCode }
					},
				};
			}

			var metadata = await _repository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = ClassifierType.EntityTypeCode,
				EntityUid = type.Uid,
				IsActive = true
			}, cancellationToken);

			DataView result;

			if (metadata.Rows.Count > 0)
			{
				result = new DataView { Fields = metadata.Rows };

				foreach (var field in result.Fields)
				{
					if (field.System == false)
					{
						field.Key = FieldKey.FormatFullKey(field.Key);
					}
				}
			}
			else
			{
				// todo: remove
				result = await _metadataProvider.GetView("Classifier/" + type.Code);
			}

			if (commonFields != null)
			{
				result.Fields = commonFields.Union(result.Fields).ToArray();
			}

			return result;
		}
	}
}
