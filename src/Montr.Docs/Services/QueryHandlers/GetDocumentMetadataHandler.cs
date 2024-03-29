﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Docs.Queries;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.Docs.Services.QueryHandlers
{
	public class GetDocumentMetadataHandler : IRequestHandler<GetDocumentMetadata, DataView>
	{
		private readonly IRepository<Document> _documentRepository;
		private readonly IConfigurationProvider _configurationProvider;
		private readonly IRepository<FieldMetadata> _metadataRepository;

		public GetDocumentMetadataHandler(
			IRepository<Document> documentRepository,
			IConfigurationProvider configurationProvider,
			IRepository<FieldMetadata> metadataRepository)
		{
			_documentRepository = documentRepository;
			_configurationProvider = configurationProvider;
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

			var document = (await _documentRepository.Search(new DocumentSearchRequest
			{
				Uid = request.DocumentUid,
				SkipPaging = true
			}, cancellationToken)).Rows.Single();

			if (request.ViewId == ViewCode.DocumentPage)
			{
				result.Toolbar = await _configurationProvider.GetItems<Document, Button>(document, request.Principal);
				result.Panes = await _configurationProvider.GetItems<Document, DataPane>(document, request.Principal);
			}
			else if (request.ViewId == ViewCode.DocumentInfo)
			{
				result.Fields = new List<FieldMetadata>
				{
					new ClassifierField { Key = "documentTypeUid", Name = "Type", Required = true, Props = { TypeCode = ClassifierTypeCode.DocumentType }},
					new TextField { Key = "documentNumber", Name = "Number", Required = true },
					new DateField { Key = "documentDate", Name = "Date", Required = true },
					new TextField { Key = "name", Name = "Name", Required = true },
				};
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
				EntityTypeCode = MasterData.EntityTypeCode.Classifier,
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
