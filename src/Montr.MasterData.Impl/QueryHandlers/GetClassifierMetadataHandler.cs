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

			ICollection<FieldMetadata> commonFields = null;

			// todo: should be edited with 
			if (type.HierarchyType == HierarchyType.Groups)
			{
				commonFields = new List<FieldMetadata>
				{
					new ClassifierGroupField
					{
						Key = "parentUid",
						Name = "Группа",
						Required = true,
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
						Key = "parentUid",
						Name = "Родительский элемент",
						Props = { TypeCode = typeCode }
					},
				};
			}

			var metadata = await _metadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = ClassifierType.EntityTypeCode,
				EntityUid = type.Uid,
				IsActive = true
			}, cancellationToken);

			var dbFields = new List<string>
			{
				ExpressionHelper.GetMemberName<Classifier>(x => x.Code),
				ExpressionHelper.GetMemberName<Classifier>(x => x.Name)
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

			if (commonFields != null)
			{
				result.Fields = commonFields.Union(result.Fields).ToArray();
			}

			return result;
		}
	}
}
