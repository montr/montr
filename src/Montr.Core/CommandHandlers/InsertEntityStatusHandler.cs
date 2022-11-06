using System;
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
	public class InsertEntityStatusHandler : IRequestHandler<InsertEntityStatus, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public InsertEntityStatusHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(InsertEntityStatus request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			if (item.Uid == Guid.Empty)
			{
				item.Uid = Guid.NewGuid();
			}

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var affected = await db.GetTable<DbEntityStatus>()
						.Value(x => x.Uid, item.Uid)
						.Value(x => x.EntityTypeCode, request.EntityTypeCode)
						.Value(x => x.EntityUid, request.EntityUid)
						.Value(x => x.Code, item.Code)
						.Value(x => x.Name, item.Name)
						.Value(x => x.DisplayOrder, item.DisplayOrder)
						.InsertAsync(cancellationToken);

					scope.Commit();

					return new ApiResult { Uid = item.Uid, AffectedRows = affected };
				}
			}
		}
	}
}
