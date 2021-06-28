using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Docs.Impl.Entities;
using Montr.Docs.Models;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Docs.Impl.Services
{
	public class DbDocumentRepository : IRepository<Document>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<FieldMetadata> _fieldMetadataRepository;
		private readonly IFieldDataRepository _fieldDataRepository;

		public DbDocumentRepository(IDbContextFactory dbContextFactory,
			IRepository<FieldMetadata> fieldMetadataRepository,
			IFieldDataRepository fieldDataRepository)
		{
			_dbContextFactory = dbContextFactory;
			_fieldMetadataRepository = fieldMetadataRepository;
			_fieldDataRepository = fieldDataRepository;
		}

		public async Task<SearchResult<Document>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (DocumentSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var query = db.GetTable<DbDocument>().AsQueryable();

				if (request.Uid != null)
				{
					query = query.Where(x => x.Uid == request.Uid);
				}

				var data = await Materialize(
					query.Apply(request, x => x.DocumentDate, SortOrder.Descending), cancellationToken);

				// todo: preload fields for multiple items
				if (request.IncludeFields)
				{
					foreach (var item in data)
					{
						if (item.Uid.HasValue)
						{
							// todo: load metadata once for each document type
							var metadata = await _fieldMetadataRepository.Search(new MetadataSearchRequest
							{
								EntityTypeCode = DocumentType.EntityTypeCode,
								EntityUid = item.DocumentTypeUid,
								IsActive = true,
								SkipPaging = true
							}, cancellationToken);

							var fields = await _fieldDataRepository.Search(new FieldDataSearchRequest
							{
								Metadata = metadata.Rows,
								EntityTypeCode = Document.TypeCode,
								EntityUids = new[] { item.Uid.Value }
							}, cancellationToken);

							item.Fields = fields.Rows.SingleOrDefault();
						}
					}
				}

				return new SearchResult<Document>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}

		private static async Task<List<Document>> Materialize(IQueryable<DbDocument> query, CancellationToken cancellationToken)
		{
			return await query.Select(x => new Document
			{
				Uid = x.Uid,
				CompanyUid = x.CompanyUid,
				DocumentTypeUid = x.DocumentTypeUid,
				StatusCode = x.StatusCode,
				Direction = Enum.Parse<DocumentDirection>(x.Direction, true),
				DocumentNumber = x.DocumentNumber,
				DocumentDate = x.DocumentDate,
				Name = x.Name,
				CreatedAtUtc = x.CreatedAtUtc,
				CreatedBy = x.CreatedBy,
				ModifiedAtUtc = x.ModifiedAtUtc,
				ModifiedBy = x.ModifiedBy,
				Url = "/documents/view/" + x.Uid
			}).ToListAsync(cancellationToken);
		}
	}
}
