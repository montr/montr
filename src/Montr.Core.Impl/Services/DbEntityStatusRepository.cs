using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Impl.Entities;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.Services
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

				var data = await Materialize(
					query.Apply(request, x => x.DisplayOrder, SortOrder.Descending), cancellationToken);

				return new SearchResult<EntityStatus>
				{
					Rows = data
				};
			}
		}

		private static async Task<List<EntityStatus>> Materialize(IQueryable<DbEntityStatus> query, CancellationToken cancellationToken)
		{
			return await query.Select(x => new EntityStatus
			{
				Code = x.Code,
				Name = x.Name
			}).ToListAsync(cancellationToken);
		}
	}
}
