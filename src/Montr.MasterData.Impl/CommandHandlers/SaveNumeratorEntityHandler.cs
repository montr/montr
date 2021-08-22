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

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class SaveNumeratorEntityHandler : IRequestHandler<SaveNumeratorEntity, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<NumeratorEntity> _repository;

		public SaveNumeratorEntityHandler(IUnitOfWorkFactory unitOfWorkFactory,
			IDbContextFactory dbContextFactory, IRepository<NumeratorEntity> repository)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_repository = repository;
		}

		public async Task<ApiResult> Handle(SaveNumeratorEntity request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var result = await _repository.Search(new NumeratorEntitySearchRequest
			{
				// EntityTypeCode = item.EntityTypeCode,
				EntityUid = item.EntityUid
			}, cancellationToken);

			var existingItem = result.Rows.SingleOrDefault();

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					if (existingItem == null)
					{
						await db.GetTable<DbNumeratorEntity>()
							.Value(x => x.IsAutoNumbering, item.IsAutoNumbering)
							.Value(x => x.EntityUid, item.EntityUid)
							.Value(x => x.NumeratorUid, item.NumeratorUid)
							.InsertAsync(cancellationToken);
					}
					else
					{
						await db.GetTable<DbNumeratorEntity>()
							.Where(x => x.EntityUid == item.EntityUid)
							.Set(x => x.IsAutoNumbering, item.IsAutoNumbering)
							.Set(x => x.NumeratorUid, item.NumeratorUid)
							.UpdateAsync(cancellationToken);
					}
				}

				scope.Commit();

				return new ApiResult();
			}
		}
	}
}
