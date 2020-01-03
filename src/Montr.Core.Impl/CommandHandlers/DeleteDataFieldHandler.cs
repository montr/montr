using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Commands;
using Montr.Core.Impl.Entities;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.CommandHandlers
{
	public class DeleteDataFieldHandler : IRequestHandler<DeleteDataField, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public DeleteDataFieldHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(DeleteDataField request, CancellationToken cancellationToken)
		{
			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					affected = await db.GetTable<DbFieldMetadata>()
						.Where(x => x.EntityTypeCode == request.EntityTypeCode && request.Uids.Contains(x.Uid))
						.DeleteAsync(cancellationToken);
				}

				// todo: (события)

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
