using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;
using Montr.Tendr.Queries;

namespace Montr.Tendr.Impl.QueryHandlers
{
	public class GetInvitationListHandler : IRequestHandler<GetInvitationList, SearchResult<InvitationListItem>>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<Classifier> _classifierRepository;

		public GetInvitationListHandler(IDbContextFactory dbContextFactory, IRepository<Classifier> classifierRepository)
		{
			_dbContextFactory = dbContextFactory;
			_classifierRepository = classifierRepository;
		}

		public async Task<SearchResult<InvitationListItem>> Handle(GetInvitationList command, CancellationToken cancellationToken)
		{
			var request = command.Request ?? throw new ArgumentNullException(nameof(command.Request));

			// todo: check event belongs to user company

			using (var db = _dbContextFactory.Create())
			{
				var all =
					from i in db.GetTable<DbInvitation>()
					join c in db.GetTable<DbClassifier>() on i.CounterpartyUid equals c.Uid
					where i.EventUid == request.EventUid
					select new InvitationListItem
					{
						Uid = i.Uid,
						StatusCode = i.StatusCode,
						CounterpartyUid = i.CounterpartyUid,
						CounterpartyName = c.Name,
						Email = i.Email
					};
				
				var data = await all
					.Apply(request, x => x.CounterpartyName, SortOrder.Descending)
					.ToListAsync(cancellationToken);

				return new SearchResult<InvitationListItem>
				{
					TotalCount = all.Count(),
					Rows = data
				};
			}
		}
	}
}
