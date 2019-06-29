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

namespace Tendr.Implementation.CommandHandlers
{
	public class UpdateEventHandler : IRequestHandler<UpdateEvent>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public UpdateEventHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<Unit> Handle(UpdateEvent request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty)
				throw new InvalidOperationException("UserUid can't be empty guid.");

			var item = request.Event;

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					await db.GetTable<DbEvent>()
						.Where(x => x.Id == item.Id)
						.Set(x => x.Name, item.Name)
						.Set(x => x.Description, item.Description)
						.UpdateAsync(cancellationToken);

					scope.Commit();

					return Unit.Value;
				}
			}
		}
	}
}
