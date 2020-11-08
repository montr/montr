using System;
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
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class DeleteClassifierHandler : IRequestHandler<DeleteClassifier, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IFieldDataRepository _fieldDataRepository;

		public DeleteClassifierHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService, IFieldDataRepository fieldDataRepository)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
			_fieldDataRepository = fieldDataRepository;
		}

		public async Task<ApiResult> Handle(DeleteClassifier request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					if (type.HierarchyType == HierarchyType.Groups)
					{
						// delete link with group
						await db.GetTable<DbClassifierLink>()
							.Where(x => request.Uids.Contains(x.ItemUid))
							.DeleteAsync(cancellationToken);
					}

					affected = await db.GetTable<DbClassifier>()
						.Where(x => x.TypeUid == type.Uid && request.Uids.Contains(x.Uid))
						.DeleteAsync(cancellationToken);
				}

				// delete fields
				await _fieldDataRepository.Delete(new DeleteFieldDataRequest
				{
					EntityTypeCode = Classifier.TypeCode,
					EntityUids = request.Uids
				}, cancellationToken);

				// todo: (события)

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
