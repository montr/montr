using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Tendr.Commands;
using Tendr.Implementation.Entities;
using Tendr.Models;

namespace Tendr.Implementation.CommandHandlers
{
	public class CancelEventHandler : IRequestHandler<CancelEvent>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public CancelEventHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<Unit> Handle(CancelEvent request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty)
				throw new InvalidOperationException("User uid can't be empty guid.");

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var affected = await db.GetTable<DbEvent>()
						.Where(x => x.Id == request.EventId)
						.Set(x => x.StatusCode, EventStatusCode.Cancelled)
						.UpdateAsync(cancellationToken);

					scope.Commit();

					return Unit.Value;
				}
			}
		}
	}
}