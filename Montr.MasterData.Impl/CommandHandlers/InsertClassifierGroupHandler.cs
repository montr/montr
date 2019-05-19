﻿using System;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class InsertClassifierGroupHandler : IRequestHandler<InsertClassifierGroup, InsertClassifierGroup.Result>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		public InsertClassifierGroupHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<InsertClassifierGroup.Result> Handle(InsertClassifierGroup request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");
			if (request.CompanyUid == Guid.Empty) throw new InvalidOperationException("Company is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, request.TypeCode, cancellationToken);

			using (var scope = _unitOfWorkFactory.Create())
			{
				var itemUid = Guid.NewGuid();

				// todo: валидация и ограничения

				using (var db = _dbContextFactory.Create())
				{
					var tree = await db.GetTable<DbClassifierTree>()
						.SingleAsync(x => x.TypeUid == type.Uid && x.Code == request.TreeCode, cancellationToken);

					var validator = new ClassifierGroupValidator(db, tree);

					if (await validator.ValidateInsert(item, cancellationToken) == false)
					{
						return new InsertClassifierGroup.Result { Success = false, Errors = validator.Errors };
					}

					await db.GetTable<DbClassifierGroup>()
						.Value(x => x.Uid, itemUid)
						.Value(x => x.TreeUid, tree.Uid)
						// todo: validate parent belongs to the same tree
						.Value(x => x.ParentUid, item.ParentUid)
						.Value(x => x.Code, item.Code)
						.Value(x => x.Name, item.Name)
						.InsertAsync(cancellationToken);

					var closureTable = new ClosureTableHandler(db);

					if (await closureTable.Insert(itemUid, item.ParentUid, cancellationToken) == false)
					{
						return new InsertClassifierGroup.Result { Success = false, Errors = closureTable.Errors };
					}
				}

				// todo: (events)

				scope.Commit();

				return new InsertClassifierGroup.Result { Success = true, Uid = itemUid };
			}
		}
	}
}
