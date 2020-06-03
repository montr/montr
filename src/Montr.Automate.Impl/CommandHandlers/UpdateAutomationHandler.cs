using System;
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
	public class UpdateAutomationHandler : IRequestHandler<UpdateAutomation, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public UpdateAutomationHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async  Task<ApiResult> Handle(UpdateAutomation request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					affected = await db.GetTable<DbAutomation>()
						.Where(x => x.EntityTypeCode == request.EntityTypeCode &&
									x.EntityTypeUid == request.EntityTypeUid &&
									x.Uid == item.Uid)
						.Set(x => x.Name, item.Name)
						.Set(x => x.Description, item.Description)
						.Set(x => x.DisplayOrder, item.DisplayOrder)
						.UpdateAsync(cancellationToken);
				}

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
