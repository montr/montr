using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
	public class LoadEventListHandler : IRequestHandler<LoadEventList, DataResult<Event>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public LoadEventListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public Task<DataResult<Event>> Handle(LoadEventList command, CancellationToken cancellationToken)
		{
			var request = command.Request;

			using (var db = _dbContextFactory.Create())
			{
				var all = db.GetTable<DbEvent>();

				var date = all
					.Apply(request, "Id", SortOrder.Descending)
					.Select(x => new Event
					{
						Uid = x.Uid,
						Id = x.Id,
						ConfigCode = x.ConfigCode,
						StatusCode = x.StatusCode,
						Name = x.Name,
						Description = x.Description,
						Url = "/events/edit/" + x.Id
					})
					.ToList();

				var result = new DataResult<Event>
				{
					TotalCount = all.Count(),
					Rows = date
				};

				return Task.FromResult(result);
			}
		}
	}
}
