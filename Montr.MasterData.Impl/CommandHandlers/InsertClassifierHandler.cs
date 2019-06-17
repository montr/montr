using System;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class InsertClassifierHandler: IRequestHandler<InsertClassifier, InsertClassifier.Result>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IClassifierTypeService _classifierTypeService;

		public InsertClassifierHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IDateTimeProvider dateTimeProvider, IClassifierTypeService classifierTypeService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<InsertClassifier.Result> Handle(InsertClassifier request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");
			if (request.CompanyUid == Guid.Empty) throw new InvalidOperationException("Company is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			// var now = _dateTimeProvider.GetUtcNow();

			// todo: check company belongs to user
			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, request.TypeCode, cancellationToken);

			using (var scope = _unitOfWorkFactory.Create())
			{
				var itemUid = Guid.NewGuid();

				// todo: валидация и ограничения

				using (var db = _dbContextFactory.Create())
				{
					var validator = new ClassifierValidator(db, type);

					if (await validator.ValidateInsert(item, cancellationToken) == false)
					{
						return new InsertClassifier.Result { Success = false, Errors = validator.Errors };
					}

					// компания + todo: дата изменения
					// todo: link to selected group or root group

					await db.GetTable<DbClassifier>()
						.Value(x => x.Uid, itemUid)
						// .Value(x => x.CompanyUid, request.CompanyUid)
						.Value(x => x.TypeUid, type.Uid)
						.Value(x => x.StatusCode, ClassifierStatusCode.Active)
						.Value(x => x.Code, item.Code)
						.Value(x => x.Name, item.Name)
						.InsertAsync(cancellationToken);
				}

				// todo: (события)

				scope.Commit();

				return new InsertClassifier.Result { Success = true, Uid = itemUid };
			}
		}
	}
}
