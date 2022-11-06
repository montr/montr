using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class DeleteClassifierTreeHandler : IRequestHandler<DeleteClassifierTree, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		public DeleteClassifierTreeHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<ApiResult> Handle(DeleteClassifierTree request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					// todo: return error when trying to remove default tree
					affected = await db.GetTable<DbClassifierTree>()
						.Where(x => x.TypeUid == type.Uid && request.Uids.Contains(x.Uid)
									&& x.Code != ClassifierTree.DefaultCode)
						.DeleteAsync(cancellationToken);
				}

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
