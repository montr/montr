using System;
using System.Collections.Generic;
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
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class DeleteClassifierLinkHandler : IRequestHandler<DeleteClassifierLink, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		public DeleteClassifierLinkHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<ApiResult> Handle(DeleteClassifierLink request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;
				ApiResultError error = null;

				using (var db = _dbContextFactory.Create())
				{
					// todo: validate group and item belongs to type

					if (type.HierarchyType != HierarchyType.Groups)
					{
						throw new InvalidOperationException("Invalid classifier hierarchy type for groups operations.");
					}

					affected = await db.GetTable<DbClassifierLink>()
						.Where(x => x.GroupUid == request.GroupUid && x.ItemUid == request.ItemUid)
						.DeleteAsync(cancellationToken);

					if (affected > 0)
					{
						var root = await GetRoot(db, request.GroupUid, cancellationToken);

						if (root.Code == ClassifierTree.DefaultCode)
						{
							// todo: move to ClassifierGroupService
							await db.GetTable<DbClassifierLink>()
								.Value(x => x.GroupUid, root.Uid)
								.Value(x => x.ItemUid, request.ItemUid)
								.InsertAsync(cancellationToken);

							error = new ApiResultError
							{
								Messages = new[] { "All items should be linked to default hierarchy. Relinked item to root." }
							};
						}
					}
				}

				// todo: events

				scope.Commit();

				var result = new ApiResult { AffectedRows = affected };

				if (error != null)
				{
					result.Errors = new List<ApiResultError> { error };
				}

				return result;
			}
		}

		// todo: move to ClassifierGroupService.GetRoot()
		public static async Task<DbClassifierGroup> GetRoot(DbContext db, Guid groupUid, CancellationToken cancellationToken)
		{
			return await (
				from closure in db.GetTable<DbClassifierClosure>()
					.Where(x => x.ChildUid == groupUid)
				join maxLevel in (
					from path in db.GetTable<DbClassifierClosure>()
						.Where(x => x.ChildUid == groupUid)
					group path by path.ChildUid
					into parents
					select parents.Max(x => x.Level)) on closure.Level equals maxLevel
				join parent in db.GetTable<DbClassifierGroup>() on closure.ParentUid equals parent.Uid
				select parent
			).SingleAsync(cancellationToken);
		}
	}
}
