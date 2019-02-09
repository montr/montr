using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Metadata.Models;
using Tendr.Implementation.Entities;
using Tendr.Models;
using Tendr.Queries;

namespace Tendr.Implementation.QueryHandlers
{
	public class GetEventListHandler : IRequestHandler<GetEventList, DataResult<Event>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetEventListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<DataResult<Event>> Handle(GetEventList command, CancellationToken cancellationToken)
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

				var result = new DataResult<Event>
				{
					TotalCount = all.Count(),
					Rows = date
				};

				return result;
			}
		}
	}
}
