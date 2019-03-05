using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class InsertClassifierHandler: IRequestHandler<InsertClassifier, int>
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

		public async Task<int> Handle(InsertClassifier request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty)
				throw new InvalidOperationException("UserUid can't be empty guid.");

			// var now = _dateTimeProvider.GetUtcNow();

			var types = await _classifierTypeRepository.Search(
				new ClassifierTypeSearchRequest
				{
					CompanyUid = request.CompanyUid,
					UserUid = request.UserUid,
					Code = request.TypeCode,
					/*PageNo = 1,
					PageSize = 1*/
				}, cancellationToken);

			var type = types.Rows.Single();

			var count = 0;
			
			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					// todo: валидация и ограничения

					// компания + todo: дата изменения

					/*foreach (var item in request.Items)
					{
						// item.Uid = Guid.NewGuid();
						item.StatusCode = ClassifierStatusCode.Draft;

						await db.GetTable<DbClassifier>()
							.Value(x => x.Uid, Guid.NewGuid())
							.Value(x => x.CompanyUid, request.CompanyUid)
							.Value(x => x.TypeUid, type.Uid)
							.Value(x => x.StatusCode, item.StatusCode)
							.Value(x => x.Code, item.Code)
							.Value(x => x.Name, item.Name)
							.InsertAsync(cancellationToken);

						count++;
					}*/

					db.BulkCopy(request.Items.Select(x => new DbClassifier
					{
						Uid = Guid.NewGuid(),
						Name = x.Name,
						Code = x.Code,
						StatusCode = ClassifierStatusCode.Draft,
						CompanyUid = request.CompanyUid,
						TypeUid = type.Uid,
					}));
				}

				// todo: (события)

				scope.Commit();

				return count;
			}
		}
	}
}
