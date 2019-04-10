using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class DeleteClassifierTypeListHandler : IRequestHandler<DeleteClassifierTypeList, int>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public DeleteClassifierTypeListHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<int> Handle(DeleteClassifierTypeList request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");
			if (request.CompanyUid == Guid.Empty) throw new InvalidOperationException("Company is required.");

			// todo: check company belongs to user

			using (var scope = _unitOfWorkFactory.Create())
			{
				int result;

				using (var db = _dbContextFactory.Create())
				{
					result = await db.GetTable<DbClassifierType>()
						.Where(x => x.CompanyUid == request.CompanyUid && request.Uids.Contains(x.Uid))
						.DeleteAsync(cancellationToken);
				}

				// todo: (события)

				scope.Commit();

				return result;
			}
		}
	}
}
