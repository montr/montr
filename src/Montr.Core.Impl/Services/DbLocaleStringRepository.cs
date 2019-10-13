using System;
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
	public class DbLocaleStringRepository : IRepository<LocaleString>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbLocaleStringRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<LocaleString>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (LocaleStringSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var all = db.GetTable<DbLocaleString>()
					.Where(x => x.Locale == request.Locale && x.Module == request.Module);

				var withPaging = request.PageSize > 0;

				var paged = withPaging ? all.Apply(request, x => x.Key) : all;

				var data = await paged
					.Select(x => new LocaleString
					{
						Key = x.Key,
						Value = x.Value
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<LocaleString>
				{
					TotalCount = withPaging ? all.Count() : (int?)null,
					Rows = data
				};
			}
		}
	}
}
