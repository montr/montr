using System;
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
	public class InsertDataFieldHandler : IRequestHandler<InsertDataField, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public InsertDataFieldHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(InsertDataField request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			using (var scope = _unitOfWorkFactory.Create())
			{
				var itemUid = Guid.NewGuid();

				using (var db = _dbContextFactory.Create())
				{
					await db.GetTable<DbFieldMeta>()
						.Value(x => x.Uid, itemUid)
						.Value(x => x.EntityTypeCode, request.EntityTypeCode)
						.Value(x => x.TypeCode, item.Type)
						.Value(x => x.Key, item.Key)
						.Value(x => x.Name, item.Name)
						.Value(x => x.Description, item.Description)
						.Value(x => x.Placeholder, item.Placeholder)
						.Value(x => x.Icon, item.Icon)
						.Value(x => x.IsActive, true)
						.Value(x => x.IsSystem, false)
						.Value(x => x.IsReadonly, false)
						.Value(x => x.IsRequired, false)
						.Value(x => x.DisplayOrder, item.DisplayOrder)
						.InsertAsync(cancellationToken);

					scope.Commit();
				}

				return new ApiResult { Uid = itemUid };
			}
		}
	}
}
