using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class DeleteNumeratorHandler : IRequestHandler<DeleteNumerator, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public DeleteNumeratorHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(DeleteNumerator request, CancellationToken cancellationToken)
		{
			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					// todo: validate if usages in DbNumeratorEntity exists (?)

					// delete all counters
					await (
						from counter in db.GetTable<DbNumeratorCounter>()
						join numerator in db.GetTable<DbNumerator>() on counter.NumeratorUid equals numerator.Uid
						where request.Uids.Contains(numerator.Uid)
						select counter
					).DeleteAsync(cancellationToken);

					// delete numerator
					affected = await db.GetTable<DbNumerator>()
						.Where(x => request.Uids.Contains(x.Uid))
						.DeleteAsync(cancellationToken);
				}

				// todo: (events)

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
