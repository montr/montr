using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Entities;
using Montr.Docs.Models;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Docs.Services.Implementations
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
			var request = (DocumentSearchRequest)searchRequest;

			SearchResult<Document> result;

			using (var db = _dbContextFactory.Create())
			{
				// todo: add indexes
				var query = db.GetTable<DbDocument>().AsQueryable();

				if (request.Uid != null)
				{
					query = query.Where(x => x.Uid == request.Uid);
				}

				if (request.UserUid != null)
				{
					query = query.Where(x => x.CreatedBy == request.UserUid);
				}

				var data = await Materialize(
					query.Apply(request, x => x.DocumentDate, SortOrder.Descending), cancellationToken);

				result = new SearchResult<Document>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}

			// todo: preload fields for multiple items
			if (request.IncludeFields)
			{
				foreach (var item in result.Rows)
				{
					if (item.Uid.HasValue)
					{
						// todo: load metadata once for each document type
						var metadata = await _fieldMetadataRepository.Search(new MetadataSearchRequest
						{
							EntityTypeCode = MasterData.EntityTypeCode.Classifier,
							EntityUid = item.DocumentTypeUid,
							IsActive = true,
							SkipPaging = true
						}, cancellationToken);

						var fields = await _fieldDataRepository.Search(new FieldDataSearchRequest
						{
							Metadata = metadata.Rows,
							EntityTypeCode = EntityTypeCode.Document,
							EntityUids = new[] { item.Uid.Value }
						}, cancellationToken);

						item.Fields = fields.Rows.SingleOrDefault();
					}
				}
			}

			return result;
		}

		protected virtual async Task<List<Document>> Materialize(IQueryable<DbDocument> query, CancellationToken cancellationToken)
		{
			return await query.Select(x => new Document
			{
				Uid = x.Uid,
				CompanyUid = x.CompanyUid,
				DocumentTypeUid = x.DocumentTypeUid,
				StatusCode = x.StatusCode,
				Direction = Enum.Parse<DocumentDirection>(x.Direction, true),
				DocumentDate = x.DocumentDate,
				DocumentNumber = x.DocumentNumber,
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