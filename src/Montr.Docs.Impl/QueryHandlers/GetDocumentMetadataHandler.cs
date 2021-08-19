using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
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
		private readonly IConfigurationService _configurationService;
		private readonly IRepository<FieldMetadata> _metadataRepository;

		public GetDocumentMetadataHandler(
			IDocumentService documentService,
			IConfigurationService configurationService,
			IRepository<FieldMetadata> metadataRepository)
		{
			_documentService = documentService;
			_configurationService = configurationService;
			_metadataRepository = metadataRepository;
		}

		public async Task<DataView> Handle(GetDocumentMetadata request, CancellationToken cancellationToken)
		{
			var result = new DataView();

			if (request.ViewId == ViewCode.DocumentList)
			{
				result.Columns = new List<DataColumn>
				{
					new() { Key = "documentNumber", Name = "Номер", Sortable = true, UrlProperty = "url", Width = 50 },
					new() { Key = "documentDate", Name = "Дата", Type = "datetime", Sortable = true, UrlProperty = "url", Width = 100, DefaultSortOrder = SortOrder.Descending },
					// new() { Key = "direction", Name = "Направление", UrlProperty = "url", Width = 30 },
					new() { Key = "name", Name = "Наименование", Width = 250 },
					// new() { Key = "configCode", Name = "Тип", Sortable = true, Width = 100 },
					new() { Key = "statusCode", Template = "status", Name = "Статус", Sortable = true, UrlProperty = "url", Width = 100 },
				};

				return result;
			}

			var document = (await _documentService.Search(new DocumentSearchRequest
			{
				UserUid = request.UserUid,
				Uid = request.DocumentUid,
				SkipPaging = true
			}, cancellationToken)).Rows.Single();

			if (request.ViewId == ViewCode.DocumentPage)
			{
				result.Toolbar = await _configurationService.GetItems<Document, Button>(document, request.Principal);
				result.Panes = await _configurationService.GetItems<Document, DataPane>(document, request.Principal);
			}
			else if (request.ViewId == ViewCode.DocumentForm)
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
	}
}
