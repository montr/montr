using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Automate.Commands;
using Montr.Automate.Impl.Entities;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Automate.Impl.CommandHandlers
{
	public class DeleteAutomationHandler : IRequestHandler<DeleteAutomation, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public DeleteAutomationHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(DeleteAutomation request, CancellationToken cancellationToken)
		{
			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					affected = await db.GetTable<DbAutomation>()
						.Where(x => x.EntityTypeCode == request.EntityTypeCode &&
									x.EntityTypeUid == request.EntityTypeUid &&
									request.Uids.Contains(x.Uid))
						.DeleteAsync(cancellationToken);
				}

				// todo: (события)

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
