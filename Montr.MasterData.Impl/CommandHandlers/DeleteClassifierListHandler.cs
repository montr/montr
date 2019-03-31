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
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class DeleteClassifierListHandler : IRequestHandler<DeleteClassifierList, int>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IRepository<ClassifierType> _classifierTypeRepository;

		public DeleteClassifierListHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService, IRepository<ClassifierType> classifierTypeRepository)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
			_classifierTypeRepository = classifierTypeRepository;
		}

		public async Task<int> Handle(DeleteClassifierList request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User uid can't be empty guid.");

			// todo: check company belongs to user
			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, request.TypeCode, cancellationToken);

			/*var types = await _classifierTypeRepository.Search(
				new ClassifierTypeSearchRequest
				{
					CompanyUid = request.CompanyUid,
					UserUid = request.UserUid,
					Code = request.TypeCode
				}, cancellationToken);

			var type = types.Rows.Single();*/

			using (var scope = _unitOfWorkFactory.Create())
			{
				int result;

				using (var db = _dbContextFactory.Create())
				{
					result = await db.GetTable<DbClassifier>()
						.Where(x => x.TypeUid == type.Uid &&
									request.Uids.Contains(x.Uid))
						.DeleteAsync(cancellationToken);
				}

				// todo: (события)

				scope.Commit();

				return result;
			}
		}
	}
}
