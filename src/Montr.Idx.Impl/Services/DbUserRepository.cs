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
	public class DbUserRepository : IRepository<User>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbUserRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<User>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (UserSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				IQueryable<DbUser> query =
					from user in db.GetTable<DbUser>()
					select user;

				var data = await Materialize(
					query.Apply(request, x => x.UserName), cancellationToken);

				return new SearchResult<User>
				{
					TotalCount = query.Count(),
					Rows = data
				};
			}
		}

		private static async Task<IList<User>> Materialize(IQueryable<DbUser> query, CancellationToken cancellationToken)
		{
			return await query
				.Select(x => new User
				{
					Uid = x.Id,
					UserName = x.UserName,
					FirstName = x.FirstName,
					LastName = x.LastName,
					Email = x.Email,
					PhoneNumber = x.PhoneNumber,
					Url = $"/users/{x.Id}"
				})
				.ToListAsync(cancellationToken);
		}
	}
}
