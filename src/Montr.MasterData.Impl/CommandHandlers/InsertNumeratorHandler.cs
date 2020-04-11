using System;
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
	public class InsertNumeratorHandler : IRequestHandler<InsertNumerator, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public InsertNumeratorHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(InsertNumerator request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var itemUid = Guid.NewGuid();

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					await db.GetTable<DbNumerator>()
						.Value(x => x.Uid, itemUid)
						.Value(x => x.EntityTypeCode, item.EntityTypeCode)
						.Value(x => x.Name, item.Name)
						.Value(x => x.Pattern, item.Pattern)
						.Value(x => x.Periodicity, item.Periodicity.ToString())
						.Value(x => x.IsActive, item.IsActive)
						.Value(x => x.IsSystem, item.IsSystem)
						.InsertAsync(cancellationToken);
				}

				scope.Commit();

				return new ApiResult { Uid = itemUid };
			}
		}
	}
}
