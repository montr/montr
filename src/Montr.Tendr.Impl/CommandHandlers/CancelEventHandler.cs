using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Tendr.Commands;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;

namespace Montr.Tendr.Impl.CommandHandlers
{
	public class CancelEventHandler : IRequestHandler<CancelEvent, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public CancelEventHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(CancelEvent request, CancellationToken cancellationToken)
		{
			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var affected = await db.GetTable<DbEvent>()
						.Where(x => x.CompanyUid == request.CompanyUid && x.Uid == request.Uid)
						.Set(x => x.StatusCode, EventStatusCode.Cancelled)
						.UpdateAsync(cancellationToken);

					scope.Commit();

					return new ApiResult { AffectedRows = affected };
				}
			}
		}
	}
}
