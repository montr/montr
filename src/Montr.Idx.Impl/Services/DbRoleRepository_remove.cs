using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;

namespace Montr.Idx.Impl.Services
{
	public class DbRoleRepository_remove : IRepository<Role>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbRoleRepository_remove(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<Role>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (RoleSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var query =
					from role in db.GetTable<DbRole>()
					select role;

				if (request.Name != null)
				{
					query = query.Where(x => x.Name == request.Name);
				}

				var data = await Materialize(
					query.Apply(request, x => x.Name), cancellationToken);

				return new SearchResult<Role>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}

		private static async Task<IList<Role>> Materialize(IQueryable<DbRole> query, CancellationToken cancellationToken)
		{
			return await query
				.Select(x => new Role
				{
					Uid = x.Id,
					Name = x.Name,
					Url = $"/roles/edit/{x.Id}"
				})
				.ToListAsync(cancellationToken);
		}
	}
}
