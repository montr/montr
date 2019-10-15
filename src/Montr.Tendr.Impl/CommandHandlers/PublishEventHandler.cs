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
using Montr.Tendr.Models;
using Montr.Worker.Services;

namespace Montr.Tendr.Impl.CommandHandlers
{
	public class PublishEventHandler : IRequestHandler<PublishEvent, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IBackgroundJobManager _jobManager;

		public PublishEventHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory, IBackgroundJobManager jobManager)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_jobManager = jobManager;
		}

		public async Task<ApiResult> Handle(PublishEvent request, CancellationToken cancellationToken)
		{
			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					affected = await db.GetTable<DbEvent>()
						.Where(x => x.CompanyUid == request.CompanyUid && x.Uid == request.Uid)
						.Set(x => x.StatusCode, EventStatusCode.Published)
						.UpdateAsync(cancellationToken);
				}

				_jobManager.Enqueue<IMediator>(
					x => x.Send(new SendInvitations { EventUid = request.Uid }, cancellationToken));

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
