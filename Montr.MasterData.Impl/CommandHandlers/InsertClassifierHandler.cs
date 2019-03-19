using System;
using System.Linq;
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
		private readonly IRepository<ClassifierType> _classifierTypeRepository;

		public InsertClassifierHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IDateTimeProvider dateTimeProvider, IRepository<ClassifierType> classifierTypeRepository)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
			_classifierTypeRepository = classifierTypeRepository;
		}

		public async Task<Guid> Handle(InsertClassifier request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty)
				throw new InvalidOperationException("UserUid can't be empty guid.");

			// var now = _dateTimeProvider.GetUtcNow();

			var types = await _classifierTypeRepository.Search(
				new ClassifierTypeSearchRequest
				{
					CompanyUid = request.CompanyUid,
					UserUid = request.UserUid,
					Code = request.TypeCode
				}, cancellationToken);

			var type = types.Rows.Single();

			var item = request.Item;

			using (var scope = _unitOfWorkFactory.Create())
			{
				var itemUid = Guid.NewGuid();

				// todo: валидация и ограничения

				using (var db = _dbContextFactory.Create())
				{
					// компания + todo: дата изменения

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

				return itemUid;
			}
		}
	}
}
