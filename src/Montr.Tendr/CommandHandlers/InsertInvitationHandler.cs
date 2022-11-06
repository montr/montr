using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Tendr.Commands;
using Montr.Tendr.Entities;
using Montr.Tendr.Models;

namespace Montr.Tendr.CommandHandlers
{
	public class InsertInvitationHandler : IRequestHandler<InsertInvitation, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly INamedServiceFactory<IClassifierRepository> _repositoryFactory;

		// todo: use INamedServiceFactory<IClassifierRepository> repositoryFactory
		public InsertInvitationHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			INamedServiceFactory<IClassifierRepository> repositoryFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_repositoryFactory = repositoryFactory;
		}

		public async Task<ApiResult> Handle(InsertInvitation request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");
			if (request.CompanyUid == Guid.Empty) throw new InvalidOperationException("Company is required.");

			// todo: check company belongs to user
			// todo: check event belongs to company
			var typeCode = "counterparty"; // todo: use settings

			var items = request.Items ?? throw new ArgumentNullException(nameof(request.Items));

			var classifierRepository = _repositoryFactory.GetNamedOrDefaultService(typeCode);

			using (var scope = _unitOfWorkFactory.Create())
			{
				var counterparties = (await classifierRepository.Search(new ClassifierSearchRequest
				{
					PageSize = 100, // todo: remove limit?
					TypeCode = typeCode,
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
								.Value(x => x.Email, item.Email)
								.InsertAsync(cancellationToken);

							affectedRows++;
						}
					}

					scope.Commit();
				}

				return new ApiResult { AffectedRows = affectedRows };
			}
		}
	}
}
