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

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class InsertClassifierHandler: IRequestHandler<InsertClassifier, Guid>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;

		public InsertClassifierHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IDateTimeProvider dateTimeProvider)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
		}

		public async Task<Guid> Handle(InsertClassifier request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty)
				throw new InvalidOperationException("UserUid can't be empty guid.");

			var now = _dateTimeProvider.GetUtcNow();

			var item = request.Item;

			item.Uid = Guid.NewGuid();
			item.StatusCode = ClassifierStatusCode.Draft;

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					// todo: валидация и ограничения

					// компания + todo: дата изменения
					await db.GetTable<DbClassifier>()
						.Value(x => x.Uid, item.Uid)
						.Value(x => x.CompanyUid, item.CompanyUid)
						.Value(x => x.ConfigCode, item.ConfigCode)
						.Value(x => x.StatusCode, item.StatusCode)
						.Value(x => x.Code, item.Code)
						.Value(x => x.Name, item.Name)
						.InsertAsync(cancellationToken);
				}

				// todo: (события)

				scope.Commit();

				return item.Uid;
			}
		}
	}
}
