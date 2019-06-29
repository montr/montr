using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Data.Linq2Db;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;
using Montr.Tendr.Queries;

namespace Montr.Tendr.Impl.QueryHandlers
{
	public class GetEventHandler : IRequestHandler<GetEvent, Event>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetEventHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<Event> Handle(GetEvent command, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				var result = await db.GetTable<DbEvent>()
					.Where(x => x.Id == command.EventId)
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
					.SingleAsync(cancellationToken);

				return result;
			}
		}
	}
}
