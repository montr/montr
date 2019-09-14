using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Models;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;
using Montr.Tendr.Queries;

namespace Montr.Tendr.Impl.QueryHandlers
{
	public class GetInvitationListHandler : IRequestHandler<GetInvitationList, SearchResult<Invitation>>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<Classifier> _classifierRepository;

		public GetInvitationListHandler(IDbContextFactory dbContextFactory, IRepository<Classifier> classifierRepository)
		{
			_dbContextFactory = dbContextFactory;
			_classifierRepository = classifierRepository;
		}

		public async Task<SearchResult<Invitation>> Handle(GetInvitationList command, CancellationToken cancellationToken)
		{
			var request = command.Request ?? throw new ArgumentNullException(nameof(command.Request));

			SearchResult<Invitation> result;

			using (var db = _dbContextFactory.Create())
			{
				var all = db.GetTable<DbInvitation>()
					.Where(x => x.EventUid == request.EventUid);

				var data = await all
					.Apply(request, x => x.Uid, SortOrder.Descending)
					.Select(x => new Invitation
					{
						Uid = x.Uid,
						StatusCode = x.StatusCode,
						CounterpartyUid = x.CounterpartyUid
					})
					.ToListAsync(cancellationToken);

				result = new SearchResult<Invitation>
				{
					TotalCount = all.Count(),
					Rows = data
				};
			}

			// todo: join in one query
			var counterparties = (await _classifierRepository.Search(new ClassifierSearchRequest
			{
				CompanyUid = request.CompanyUid,
				TypeCode = "counterparty", // todo: use settings
				Uids = result.Rows.Select(x => x.CounterpartyUid).Distinct().ToArray()
			}, cancellationToken)).Rows.ToDictionary(x => x.Uid);

			foreach (var invitation in result.Rows)
			{
				if (counterparties.TryGetValue(invitation.CounterpartyUid, out Classifier counterparty))
				{
					invitation.Counterparty = counterparty;
				}
			}

			return result;
		}
	}
}
