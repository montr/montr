using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Tendr.Commands;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;

namespace Montr.Tendr.Impl.CommandHandlers
{
	public class PublishEventHandler : IRequestHandler<PublishEvent>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public PublishEventHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<Unit> Handle(PublishEvent request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty)
				throw new InvalidOperationException("User uid can't be empty guid.");

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var affected = await db.GetTable<DbEvent>()
						.Where(x => x.Id == request.EventId)
						.Set(x => x.StatusCode, EventStatusCode.Published)
						.UpdateAsync(cancellationToken);

					scope.Commit();

					return Unit.Value;
				}
			}
		}
	}
}
