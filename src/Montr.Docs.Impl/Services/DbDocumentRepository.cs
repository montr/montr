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
using Montr.Docs.Services;

namespace Montr.Docs.Impl.Services
{
	// todo: merge with IRepository<Document>?
	public class DbDocumentRepository : IDocumentRepository
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbDocumentRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task Create(Document document, CancellationToken cancellationToken)
		{
			if (document.Uid == Guid.Empty)
				document.Uid = Guid.NewGuid();

			if (document.StatusCode == null)
				document.StatusCode = DocumentStatusCode.Draft;

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbDocument>()
					.Value(x => x.Uid, document.Uid)
					.Value(x => x.CompanyUid, document.CompanyUid)
					.Value(x => x.ConfigCode, document.ConfigCode)
					.Value(x => x.StatusCode, document.StatusCode)
					.Value(x => x.Direction, document.Direction.ToString())
					.Value(x => x.DocumentNumber, document.DocumentNumber)
					.Value(x => x.DocumentDate, document.DocumentDate)
					.Value(x => x.Name, document.Name)
					.InsertAsync(cancellationToken);
			}
		}

		public async Task<SearchResult<Document>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (DocumentSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var query = from c in db.GetTable<DbDocument>()
					select c;

				var data = await Materialize(
					query.Apply(request, x => x.ConfigCode), cancellationToken);

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
				ConfigCode = x.ConfigCode,
				StatusCode = x.StatusCode,
				Direction = Enum.Parse<DocumentDirection>(x.Direction),
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
