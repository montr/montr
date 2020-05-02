using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Docs.Services;
using Montr.Kompany.Models;
using Montr.Kompany.Queries;
using Montr.Metadata.Models;

namespace Montr.Kompany.Impl.QueryHandlers
{
	// todo: why named GetCompanyMetadata, if returns metadata of registration document?
	public class GetCompanyMetadataHandler : IRequestHandler<GetCompanyMetadata, DataView>
	{
		private readonly IDocumentTypeService _documentTypeService;
		private readonly IRepository<FieldMetadata> _metadataRepository;

		public GetCompanyMetadataHandler(
			IDocumentTypeService documentTypeService,
			IRepository<FieldMetadata> metadataRepository)
		{
			_documentTypeService = documentTypeService;
			_metadataRepository = metadataRepository;
		}

		public async Task<DataView> Handle(GetCompanyMetadata request, CancellationToken cancellationToken)
		{
			var documentType = await _documentTypeService.Get(DocumentTypes.CompanyRegistrationRequest, cancellationToken);

			var metadata = await _metadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = DocumentType.EntityTypeCode,
				// ReSharper disable once PossibleInvalidOperationException
				EntityUid = documentType.Uid.Value,
				IsActive = true,
				SkipPaging = true
			}, cancellationToken);

			var dbFields = new List<string>
			{
				nameof(Company.Name)
			};

			var result = new DataView { Fields = metadata.Rows };

			foreach (var field in result.Fields)
			{
				if (dbFields.Contains(field.Key, StringComparer.InvariantCultureIgnoreCase) == false)
				{
					field.Key = FieldKey.FormatFullKey(field.Key);
				}
			}

			return result;
		}
	}
}
