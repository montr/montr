using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Tendr.Entities;
using Montr.Tendr.Models;
using Montr.Tendr.Queries;

namespace Montr.Tendr.QueryHandlers
{
	public class GetEventListHandler : IRequestHandler<GetEventList, SearchResult<Event>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetEventListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<Event>> Handle(GetEventList request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			using (var db = _dbContextFactory.Create())
			{
				var all = db.GetTable<DbEvent>()
					.Where(x => x.CompanyUid == request.CompanyUid
					            && x.IsTemplate == request.IsTemplate);

				var data = await all
					.Apply(request, x => x.Id, SortOrder.Descending)
					.Select(x => new Event
					{
						Uid = x.Uid,
						Id = x.Id,
						ConfigCode = x.ConfigCode,
						StatusCode = x.StatusCode,
						Name = x.Name,
						Description = x.Description,
						Url = "/events/edit/" + x.Uid
					})
					.ToListAsync(cancellationToken);

				var result = new SearchResult<Event>
				{
					TotalCount = all.GetTotalCount(request),
					Rows = data
				};

				return result;
			}
		}
	}
}
