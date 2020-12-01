﻿using System;
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
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class UpdateClassifierGroupHandler : IRequestHandler<UpdateClassifierGroup, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		public UpdateClassifierGroupHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<ApiResult> Handle(UpdateClassifierGroup request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			// todo: validate required fields
			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					/*var tree = await db.GetTable<DbClassifierTree>()
						.SingleAsync(x => x.TypeUid == type.Uid && x.Code == request.TreeCode, cancellationToken);*/

					var validator = new ClassifierGroupValidator(db);

					if (await validator.ValidateUpdate(item, cancellationToken) == false)
					{
						return new ApiResult { Success = false, Errors = validator.Errors };
					}

					var closureTable = new ClosureTableHandler(db, type);

					if (await closureTable.Update(item.Uid, item.ParentUid, cancellationToken) == false)
					{
						return new ApiResult { Success = false, Errors = closureTable.Errors };
					}

					affected = await db.GetTable<DbClassifierGroup>()
						.Where(x => x.Uid == item.Uid)
						.Set(x => x.ParentUid, item.ParentUid)
						.Set(x => x.Code, item.Code)
						.Set(x => x.Name, item.Name)
						.UpdateAsync(cancellationToken);
				}

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
