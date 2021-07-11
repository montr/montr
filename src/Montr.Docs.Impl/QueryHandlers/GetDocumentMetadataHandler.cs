using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Docs.Queries;
using Montr.MasterData;
using Montr.Metadata.Models;

namespace Montr.Docs.Impl.QueryHandlers
{
	public class GetDocumentMetadataHandler : IRequestHandler<GetDocumentMetadata, DataView>
	{
		private readonly IRepository<FieldMetadata> _metadataRepository;

		public GetDocumentMetadataHandler(IRepository<FieldMetadata> metadataRepository)
		{
			_metadataRepository = metadataRepository;
		}

		public async Task<DataView> Handle(GetDocumentMetadata request, CancellationToken cancellationToken)
		{
			var result = new DataView();

			if (request.ViewId == ViewCode.DocumentTabs)
			{
				result.Panes = GetDocumentTabs();
			}
			else
			{
				result.Fields = await GetDocumentFormMetadata(request, cancellationToken);
			}

			return result;
		}

		private async Task<IList<FieldMetadata>> GetDocumentFormMetadata(GetDocumentMetadata request, CancellationToken cancellationToken)
		{
			var documentTypeUid = request.DocumentTypeUid ?? throw new ArgumentNullException(
				nameof(request), $"{nameof(request.DocumentTypeUid)} is required to get document metadata.");

			// todo: separate document type metadata and document type (company registration request) form
			var metadata = await _metadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = EntityTypeCode.Classifier,
				EntityUid = documentTypeUid,
				IsActive = true,
				SkipPaging = true
			}, cancellationToken);

			// todo: remove for form
			var dbFields = new List<string>
			{
				nameof(Document.DocumentDate),
				nameof(Document.DocumentNumber),
				nameof(Document.Name)
			};

			var result = metadata.Rows;

			foreach (var field in result)
			{
				if (dbFields.Contains(field.Key, StringComparer.InvariantCultureIgnoreCase) == false)
				{
					field.Key = FieldKey.FormatFullKey(field.Key);
				}
			}

			return result;
		}

		private static DataPane[] GetDocumentTabs()
		{
			return new DataPane[]
			{
				new() { Key = "form", Name = "Questionnaire", Component = "pane_view_document_form" },
				new() { Key = "history", Name = "History", Icon = "eye" }
			};
		}
	}
}
