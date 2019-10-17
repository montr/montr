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
	public class UpdateEventHandler : IRequestHandler<UpdateEvent, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public UpdateEventHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(UpdateEvent request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			// todo: check event belongs to user company

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var affected = await db.GetTable<DbEvent>()
						.Where(x => x.Uid == item.Uid)
						.Set(x => x.Name, item.Name)
						.Set(x => x.Description, item.Description)
						.UpdateAsync(cancellationToken);

					scope.Commit();

					return new ApiResult { AffectedRows = affected };
				}
			}
		}
	}
}
