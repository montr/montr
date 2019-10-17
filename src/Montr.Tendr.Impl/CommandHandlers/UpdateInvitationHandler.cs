using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Tendr.Commands;
using Montr.Tendr.Impl.Entities;

namespace Montr.Tendr.Impl.CommandHandlers
{
	public class UpdateInvitationHandler : IRequestHandler<UpdateInvitation, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public UpdateInvitationHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(UpdateInvitation request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			// todo: check invitation belongs to user company

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var validator = new InvitationValidator(db, request.EventUid);

					if (await validator.ValidateUpdate(item, cancellationToken) == false)
					{
						return new ApiResult { Success = false, Errors = validator.Errors };
					}

					var affected = await db.GetTable<DbInvitation>()
						.Where(x => x.Uid == item.Uid)
						.Set(x => x.CounterpartyUid, item.CounterpartyUid)
						.Set(x => x.Email, item.Email)
						.UpdateAsync(cancellationToken);

					scope.Commit();

					return new ApiResult { AffectedRows = affected  };
				}
			}
		}
	}
}
