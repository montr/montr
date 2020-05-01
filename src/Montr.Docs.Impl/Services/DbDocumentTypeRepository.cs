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

namespace Montr.Docs.Impl.Services
{
	public class DbDocumentTypeRepository : IRepository<DocumentType>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbDocumentTypeRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}
		
		public async Task<SearchResult<DocumentType>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (DocumentTypeSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var query = db.GetTable<DbDocumentType>().AsQueryable();
				
				if (request.Uid != null)
				{
					query = query.Where(x => x.Uid == request.Uid);
				}

				var data = await Materialize(
					query.Apply(request, x => x.Code, SortOrder.Descending), cancellationToken);
				
				return new SearchResult<DocumentType>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}
		
		private static async Task<List<DocumentType>> Materialize(IQueryable<DbDocumentType> query, CancellationToken cancellationToken)
		{
			return await query.Select(x => new DocumentType
			{
				Uid = x.Uid,
				Code = x.Code,
				Name = x.Name,
				Description = x.Description,
				Url = "/documentTypes/view/" + x.Uid
			}).ToListAsync(cancellationToken);
		}
	}
}
