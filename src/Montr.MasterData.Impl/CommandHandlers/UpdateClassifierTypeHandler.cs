﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class UpdateClassifierTypeHandler : IRequestHandler<UpdateClassifierType, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public UpdateClassifierTypeHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(UpdateClassifierType request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");
			if (request.CompanyUid == Guid.Empty) throw new InvalidOperationException("Company is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			// todo: check type belongs to company

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var validator = new ClassifierTypeValidator(db);

					if (await validator.ValidateUpdate(item, cancellationToken) == false)
					{
						return new ApiResult { Success = false, Errors = validator.Errors };
					}

					await db.GetTable<DbClassifierType>()
						.Where(x => x.Uid == item.Uid && x.CompanyUid == request.CompanyUid)
						.Set(x => x.Code, item.Code)
						.Set(x => x.Name, item.Name)
						.Set(x => x.Description, item.Description)
						.Set(x => x.HierarchyType, item.HierarchyType.ToString())
						.UpdateAsync(cancellationToken);

					if (item.HierarchyType == HierarchyType.Groups)
					{
						var root = db.GetTable<DbClassifierGroup>()
							.SingleOrDefault(x =>
								x.TypeUid == item.Uid &&
								x.Code == ClassifierTree.DefaultCode);

						if (root == null)
						{
							var treeUid = Guid.NewGuid();

							await db.GetTable<DbClassifierGroup>()
								.Value(x => x.Uid, treeUid)
								.Value(x => x.TypeUid, item.Uid)
								.Value(x => x.Code, ClassifierTree.DefaultCode)
								.Value(x => x.Name, item.Name)
								.InsertAsync(cancellationToken);

							// todo: validate with ClassifierGroupValidator or reuse common service with InsertClassifierGroupHandler
							var closureTable = new ClosureTableHandler(db);

							if (await closureTable.Insert(treeUid, null, cancellationToken) == false)
							{
								return new ApiResult { Success = false, Errors = closureTable.Errors };
							}
						}
					}
				}

				// todo: (events)

				scope.Commit();

				return new ApiResult { Success = true };
			}
		}
	}
}
