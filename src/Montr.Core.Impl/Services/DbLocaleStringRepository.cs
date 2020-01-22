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
					.Where(x => (request.Locale == null || x.Locale == request.Locale)
					            && (request.Module == null || x.Module == request.Module));

				var paged = all.Apply(request, x => x.Key);

				var data = await paged
					.Select(x => new LocaleString
					{
						Locale = x.Locale,
						Module = x.Module,
						Key = x.Key,
						Value = x.Value
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<LocaleString>
				{
					TotalCount = all.GetTotalCount(request),
					Rows = data
				};
			}
		}
	}
}
