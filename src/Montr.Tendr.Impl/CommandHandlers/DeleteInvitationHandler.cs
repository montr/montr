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
	public class DeleteInvitationHandler : IRequestHandler<DeleteInvitation, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public DeleteInvitationHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(DeleteInvitation request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");
			if (request.CompanyUid == Guid.Empty) throw new InvalidOperationException("Company is required.");

			// todo: check company & invitation belongs to user
			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					affected = await db.GetTable<DbInvitation>()
						.Where(x => request.Uids.Contains(x.Uid))
						.DeleteAsync(cancellationToken);
				}

				// todo: (события)

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
