using System;
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
	public class UpdateNumeratorHandler : IRequestHandler<UpdateNumerator, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public UpdateNumeratorHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(UpdateNumerator request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					await db.GetTable<DbNumerator>()
						.Where(x => x.Uid == item.Uid)
						// .Set(x => x.Name, item.Name)
						.Set(x => x.Pattern, item.Pattern)
						.Set(x => x.Periodicity, item.Periodicity.ToString())
						// .Set(x => x.IsActive, item.IsActive)
						// .Set(x => x.IsSystem, item.IsSystem)
						.UpdateAsync(cancellationToken);
				}

				scope.Commit();

				return new ApiResult();
			}
		}
	}
}
