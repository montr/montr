using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Models;
using Montr.Metadata.Models;
using Montr.Tendr.Commands;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;

namespace Montr.Tendr.Impl.CommandHandlers
{
	public class InsertInvitationHandler : IRequestHandler<InsertInvitation, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<Classifier> _classifierRepository;

		public InsertInvitationHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IRepository<Classifier> classifierRepository)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_classifierRepository = classifierRepository;
		}

		public async Task<ApiResult> Handle(InsertInvitation request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");
			if (request.CompanyUid == Guid.Empty) throw new InvalidOperationException("Company is required.");

			// todo: check company belongs to user
			// todo: check event belongs to company

			var items = request.Items ?? throw new ArgumentNullException(nameof(request.Items));

			using (var scope = _unitOfWorkFactory.Create())
			{
				var counterparties = (await _classifierRepository.Search(new ClassifierSearchRequest
				{
					PageSize = 100, // todo: remove limit?
					CompanyUid = request.CompanyUid,
					TypeCode = "counterparty", // todo: use settings
					Uids = items.Select(x => x.CounterpartyUid).Distinct().ToArray()
				}, cancellationToken)).Rows.Select(x => x.Uid).ToList();

				var affectedRows = 0;

				using (var db = _dbContextFactory.Create())
				{
					var existingInvitations = db.GetTable<DbInvitation>()
						.Where(x => x.EventUid == request.EventUid)
						.Select(x => x.CounterpartyUid)
						.ToList();

					foreach (var item in items)
					{
						if (counterparties.Contains(item.CounterpartyUid) &&
							existingInvitations.Contains(item.CounterpartyUid) == false)
						{
							// todo: bulk insert
							await db.GetTable<DbInvitation>()
								.Value(x => x.Uid, Guid.NewGuid())
								.Value(x => x.EventUid, request.EventUid)
								.Value(x => x.CounterpartyUid, item.CounterpartyUid)
								.Value(x => x.StatusCode, InvitationStatusCode.Draft)
								.InsertAsync(cancellationToken);

							affectedRows++;
						}
					}

					scope.Commit();
				}

				return new ApiResult { Success = true, AffectedRows = affectedRows };
			}
		}
	}
}
