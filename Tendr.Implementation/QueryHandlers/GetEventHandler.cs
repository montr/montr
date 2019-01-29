using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Data.Linq2Db;
using Tendr.Implementation.Entities;
using Tendr.Models;
using Tendr.Queries;

namespace Tendr.Implementation.QueryHandlers
{
	public class GetEventHandler : IRequestHandler<GetEvent, Event>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetEventHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public Task<Event> Handle(GetEvent command, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				var result = db.GetTable<DbEvent>()
					.Where(x => x.Id == command.EventId)
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
					.Single();

				return Task.FromResult(result);
			}
		}
	}
}
