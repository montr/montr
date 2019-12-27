using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierMetadataHandler : IRequestHandler<GetClassifierMetadata, DataView>
	{
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IMetadataProvider _metadataProvider;
		private readonly IRepository<DataField> _repository;

		public GetClassifierMetadataHandler(IClassifierTypeService classifierTypeService,
			IMetadataProvider metadataProvider, IRepository<DataField> repository)
		{
			_classifierTypeService = classifierTypeService;
			_metadataProvider = metadataProvider;
			_repository = repository;
		}

		public async Task<DataView> Handle(GetClassifierMetadata request, CancellationToken cancellationToken)
		{
			var typeCode = request.TypeCode ?? throw new ArgumentNullException(nameof(request.TypeCode));

			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, typeCode, cancellationToken);

			ICollection<DataField> commonFields = null;

			if (type.HierarchyType == HierarchyType.Groups)
			{
				commonFields = new List<DataField>
				{
					new ClassifierGroupField { Key = "parentUid", Name = "Группа", TypeCode = typeCode, TreeCode = ClassifierTree.DefaultCode, Required = true }
				};
			}
			else if (type.HierarchyType == HierarchyType.Items)
			{
				commonFields = new List<DataField>
				{
					new ClassifierGroupField { Key = "parentUid", Name = "Родительский элемент", TypeCode = typeCode },
				};
			}

			var metadata = await _repository.Search(new MetadataSearchRequest { EntityTypeCode = "classifier." + type.Code }, cancellationToken);

			DataView result;

			if (metadata.Rows.Count > 0)
			{
				result = new DataView { Fields = metadata.Rows };
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
