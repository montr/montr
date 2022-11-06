using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;
using Montr.Tendr.Queries;

namespace Montr.Tendr.Impl.QueryHandlers
{
	public class GetInvitationHandler : IRequestHandler<GetInvitation, Invitation>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetInvitationHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<Invitation> Handle(GetInvitation command, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				var result = await db.GetTable<DbInvitation>()
					.Where(x => x.Uid == command.Uid)
					.Select(x => new Invitation
					{
						Uid = x.Uid,
						StatusCode = x.StatusCode,
						CounterpartyUid = x.CounterpartyUid,
						Email = x.Email
					})
					.SingleAsync(cancellationToken);

				return result;
			}
		}
	}
}
