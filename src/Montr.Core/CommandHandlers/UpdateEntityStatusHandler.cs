using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Commands;
using Montr.Core.Entities;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Core.CommandHandlers
{
	public class UpdateEntityStatusHandler : IRequestHandler<UpdateEntityStatus, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public UpdateEntityStatusHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(UpdateEntityStatus request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var affected = await db.GetTable<DbEntityStatus>()
						.Where(x => x.EntityTypeCode == request.EntityTypeCode &&
									x.EntityUid == request.EntityUid &&
									x.Uid == item.Uid)
						.Set(x => x.DisplayOrder, item.DisplayOrder)
						.Set(x => x.Code, item.Code)
						.Set(x => x.Name, item.Name)
						.UpdateAsync(cancellationToken);

					scope.Commit();

					return new ApiResult { AffectedRows = affected };
				}
			}
		}
	}
}
