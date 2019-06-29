using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;
using Montr.Tendr.Queries;

namespace Montr.Tendr.Impl.QueryHandlers
{
	public class GetEventListHandler : IRequestHandler<GetEventList, SearchResult<Event>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetEventListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<Event>> Handle(GetEventList command, CancellationToken cancellationToken)
		{
			var request = command.Request;

			using (var db = _dbContextFactory.Create())
			{
				var all = db.GetTable<DbEvent>();

				var date = await all
					.Apply(request, x => x.Id, SortOrder.Descending)
					.Select(x => new Event
					{
						Uid = x.Uid,
						Id = x.Id,
						ConfigCode = x.ConfigCode,
						StatusCode = x.StatusCode,
						CompanyUid = x.CompanyUid,
						Name = x.Name,
						Description = x.Description,
						Url = "/events/edit/" + x.Id
					})
					.ToListAsync(cancellationToken);

				var result = new SearchResult<Event>
				{
					TotalCount = all.Count(),
					Rows = date
				};

				return result;
			}
		}
	}
}
