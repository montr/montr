using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Docs.Queries;
using Montr.Docs.Services;
using Montr.Metadata.Models;

namespace Montr.Docs.Impl.QueryHandlers
{
	public class GetDocumentMetadataHandler : IRequestHandler<GetDocumentMetadata, DataView>
	{
		private readonly IDocumentTypeService _documentTypeService;
		private readonly IRepository<FieldMetadata> _metadataRepository;

		public GetDocumentMetadataHandler(
			IDocumentTypeService documentTypeService,
			IRepository<FieldMetadata> metadataRepository)
		{
			_documentTypeService = documentTypeService;
			_metadataRepository = metadataRepository;
		}

		public async Task<DataView> Handle(GetDocumentMetadata request, CancellationToken cancellationToken)
		{
			Guid documentTypeUid;

			if (request.DocumentTypeUid == null)
			{
				var typeCode = request.TypeCode ?? throw new ArgumentNullException(nameof(request.TypeCode));

				var type = await _documentTypeService.Get(typeCode, cancellationToken);

				documentTypeUid = type.Uid;
			}
			else
			{
				documentTypeUid = request.DocumentTypeUid.Value;
			}

			var metadata = await _metadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = DocumentType.EntityTypeCode,
				EntityUid = documentTypeUid,
				IsActive = true,
				SkipPaging = true
			}, cancellationToken);

			var dbFields = new List<string>
			{
				nameof(Document.DocumentDate),
				nameof(Document.DocumentNumber),
				nameof(Document.Name)
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
