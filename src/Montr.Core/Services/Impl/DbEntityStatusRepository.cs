using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Entities;
using Montr.Core.Models;

namespace Montr.Core.Services.Impl
{
	public class DbEntityStatusRepository : IRepository<EntityStatus>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbEntityStatusRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<EntityStatus>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (EntityStatusSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var query = db.GetTable<DbEntityStatus>()
					.Where(x => x.EntityTypeCode == request.EntityTypeCode && x.EntityUid == request.EntityUid);

				if (request.Uid != null)
				{
					query = query.Where(x => x.Uid == request.Uid);
				}

				var data = await Materialize(
					query.Apply(request, x => x.DisplayOrder), cancellationToken);

				return new SearchResult<EntityStatus>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}

		private static async Task<List<EntityStatus>> Materialize(IQueryable<DbEntityStatus> query, CancellationToken cancellationToken)
		{
			return await query.Select(x => new EntityStatus
			{
				Uid = x.Uid,
				DisplayOrder = x.DisplayOrder,
				Code = x.Code,
				Name = x.Name
			}).ToListAsync(cancellationToken);
		}
	}
}
