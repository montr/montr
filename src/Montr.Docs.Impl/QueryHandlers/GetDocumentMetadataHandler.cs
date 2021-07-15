using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Docs.Queries;
using Montr.Docs.Services;
using Montr.MasterData;
using Montr.Metadata.Models;

namespace Montr.Docs.Impl.QueryHandlers
{
	public class GetDocumentMetadataHandler : IRequestHandler<GetDocumentMetadata, DataView>
	{
		private readonly IDocumentService _documentService;
		private readonly IRepository<FieldMetadata> _metadataRepository;

		public GetDocumentMetadataHandler(IDocumentService documentService, IRepository<FieldMetadata> metadataRepository)
		{
			_documentService = documentService;
			_metadataRepository = metadataRepository;
		}

		public async Task<DataView> Handle(GetDocumentMetadata request, CancellationToken cancellationToken)
		{
			var document = (await _documentService.Search(new DocumentSearchRequest
			{
				UserUid = request.UserUid,
				Uid = request.DocumentUid,
				SkipPaging = true
			}, cancellationToken)).Rows.Single();

			var result = new DataView();

			if (request.ViewId == ViewCode.DocumentTabs)
			{
				result.Panes = GetDocumentTabs(document);
			}
			else
			{
				result.Fields = await GetDocumentFormMetadata(document, cancellationToken);
			}

			return result;
		}

		private async Task<IList<FieldMetadata>> GetDocumentFormMetadata(Document document, CancellationToken cancellationToken)
		{
			// todo: separate document type metadata and document type (company registration request) form
			var metadata = await _metadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = EntityTypeCode.Classifier,
				EntityUid = document.DocumentTypeUid,
				IsActive = true,
				SkipPaging = true
			}, cancellationToken);

			var result = metadata.Rows;

			// todo: remove for form - system fields required only for custom fields
			/*var dbFields = new List<string>
			{
				nameof(Document.DocumentDate),
				nameof(Document.DocumentNumber),
				nameof(Document.Name)
			};

			foreach (var field in result)
			{
				if (dbFields.Contains(field.Key, StringComparer.InvariantCultureIgnoreCase) == false)
				{
					field.Key = FieldKey.FormatFullKey(field.Key);
				}
			}*/

			return result;
		}

		private static DataPane[] GetDocumentTabs(Document document)
		{
			// todo: create class for each component type
			var formProps = document.StatusCode == "draft" ? new { mode = "edit" } : new { mode = "view" };

			return new DataPane[]
			{
				new() { Key = "form", Name = "Form", Component = "pane_view_document_form", Props = formProps },
				new() { Key = "history", Name = "History", Icon = "eye" }
			};
		}
	}
}
