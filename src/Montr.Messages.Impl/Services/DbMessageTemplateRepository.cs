using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Messages.Impl.Entities;
using Montr.Messages.Models;

namespace Montr.Messages.Impl.Services
{
	public class DbMessageTemplateRepository : IRepository<MessageTemplate>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbMessageTemplateRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<MessageTemplate>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (MessageTemplateSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var all = db.GetTable<DbMessageTemplate>().AsQueryable();

				if (request.Code != null)
				{
					// all = all.Where(x => x.Code == request.Code);
				}

				if (request.Uid != null)
				{
					all = all.Where(x => x.Uid == request.Uid);
				}

				var paged = all.Apply(request, x => x.Subject);

				var data = await paged
					.Select(x => new MessageTemplate
					{
						Uid = x.Uid,
						Subject = x.Subject,
						Body = x.Body,
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<MessageTemplate>
				{
					TotalCount = all.GetTotalCount(request),
					Rows = data
				};
			}
		}
	}
}
