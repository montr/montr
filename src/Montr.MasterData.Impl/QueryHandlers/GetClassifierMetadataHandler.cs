using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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

		public GetClassifierMetadataHandler(IClassifierTypeService classifierTypeService, IMetadataProvider metadataProvider)
		{
			_classifierTypeService = classifierTypeService;
			_metadataProvider = metadataProvider;
		}

		public async Task<DataView> Handle(GetClassifierMetadata request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");
			if (request.CompanyUid == Guid.Empty) throw new InvalidOperationException("Company is required.");

			var typeCode = request.TypeCode ?? throw new ArgumentNullException(nameof(request.TypeCode));

			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, typeCode, cancellationToken);

			ICollection<FormField> commonFields = null;

			if (type.HierarchyType == HierarchyType.Groups)
			{
				commonFields = new List<FormField>
				{
					new ClassifierField { Key = "parentUid", Name = "Группа", TypeCode = typeCode, Required = true }
				};
			}
			else if (type.HierarchyType == HierarchyType.Items)
			{
				commonFields = new List<FormField>
				{
					new ClassifierField { Key = "parentUid", Name = "Родительский элемент", TypeCode = typeCode },
				};
			}

			var result = await _metadataProvider.GetView("Classifier/" + type.Code);

			if (commonFields != null)
			{
				result.Fields = commonFields.Union(result.Fields).ToArray();
			}

			return result;
		}
	}
}
