using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Kompany.Impl.Entities;
using Montr.Kompany.Models;

namespace Montr.Kompany.Impl.Services
{
	public class DbCompanyRepository : IRepository<Company>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbCompanyRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<Company>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (CompanySearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			var userUid = request.UserUid;

			using (var db = _dbContextFactory.Create())
			{
				var query = from c in db.GetTable<DbCompany>()
					join cu in db.GetTable<DbCompanyUser>()
						on c.Uid equals cu.CompanyUid
					orderby c.Name
					where cu.UserUid == userUid
					select c;

				var data = await Materialize(
					query.Apply(request, x => x.Name), cancellationToken);

				return new SearchResult<Company>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}

		private static async Task<List<Company>> Materialize(IQueryable<DbCompany> query, CancellationToken cancellationToken)
		{
			return await query.Select(x => new Company
			{
				Uid = x.Uid,
				ConfigCode = x.ConfigCode,
				StatusCode = x.StatusCode,
				Name = x.Name
			}).ToListAsync(cancellationToken);
		}
	}
}
