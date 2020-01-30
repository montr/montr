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
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class InsertClassifierTreeHandler : IRequestHandler<InsertClassifierTree, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		public InsertClassifierTreeHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<ApiResult> Handle(InsertClassifierTree request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var type = await _classifierTypeService.GetClassifierType(request.TypeCode, cancellationToken);

			using (var scope = _unitOfWorkFactory.Create())
			{
				var itemUid = Guid.NewGuid();

				using (var db = _dbContextFactory.Create())
				{
					// todo: company + modification data

					// insert classifier tree
					await db.GetTable<DbClassifierTree>()
						.Value(x => x.Uid, itemUid)
						.Value(x => x.TypeUid, type.Uid)
						.Value(x => x.Code, item.Code)
						.Value(x => x.Name, item.Name)
						.InsertAsync(cancellationToken);

					// todo: events

					scope.Commit();
				}

				return new ApiResult { Uid = itemUid };
			}
		}
	}
}
