using System;
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
	public class InsertAutomationHandler : IRequestHandler<InsertAutomation, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public InsertAutomationHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(InsertAutomation request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var itemUid = Guid.NewGuid();

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					await db.GetTable<DbAutomation>()
						.Value(x => x.Uid, itemUid)
						.Value(x => x.EntityTypeCode, request.EntityTypeCode)
						.Value(x => x.EntityTypeUid, request.EntityTypeUid)
						.Value(x => x.TypeCode, "trigger") // todo: ask user
						.Value(x => x.Name, item.Name)
						.Value(x => x.Description, item.Description)
						.Value(x => x.IsActive, true)
						.Value(x => x.IsSystem, item.System)
						.Value(x => x.DisplayOrder, item.DisplayOrder)
						.InsertAsync(cancellationToken);
				}

				scope.Commit();

				return new ApiResult { Uid = itemUid };
			}
		}
	}
}
