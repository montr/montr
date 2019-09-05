using System;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
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

		public InsertInvitationHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(InsertInvitation request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");
			if (request.CompanyUid == Guid.Empty) throw new InvalidOperationException("Company is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			// todo: check company belongs to user
			// todo: check event belongs to company

			using (var scope = _unitOfWorkFactory.Create())
			{
				var itemUid = Guid.NewGuid();

				using (var db = _dbContextFactory.Create())
				{
					await db.GetTable<DbInvitation>()
						.Value(x => x.Uid, itemUid)
						.Value(x => x.EventUid, item.EventUid)
						.Value(x => x.CounterpartyUid, item.CounterpartyUid)
						.Value(x => x.StatusCode, InvitationStatusCode.Draft)
						.InsertAsync(cancellationToken);

					scope.Commit();
				}

				return new ApiResult { Success = true, Uid = itemUid };
			}
		}
	}
}
