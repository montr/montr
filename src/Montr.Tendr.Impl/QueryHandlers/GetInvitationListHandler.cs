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
	public class GetInvitationListHandler : IRequestHandler<GetInvitationList, SearchResult<Invitation>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetInvitationListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<Invitation>> Handle(GetInvitationList command, CancellationToken cancellationToken)
		{
			var request = command.Request;

			using (var db = _dbContextFactory.Create())
			{
				var all = db.GetTable<DbInvitation>();

				var data = await all
					.Apply(request, x => x.Uid, SortOrder.Descending)
					.Select(x => new Invitation
					{
						Uid = x.Uid,
						CompanyUid = x.CompanyUid
					})
					.ToListAsync(cancellationToken);

				var result = new SearchResult<Invitation>
				{
					TotalCount = all.Count(),
					Rows = data
				};

				return result;
			}
		}
	}
}