using System;
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
	public class UpdateDataFieldHandler : IRequestHandler<UpdateDataField, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IJsonSerializer _jsonSerializer;

		public UpdateDataFieldHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory, IJsonSerializer jsonSerializer)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_jsonSerializer = jsonSerializer;
		}

		public async Task<ApiResult> Handle(UpdateDataField request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var properties = item.GetProperties();

			var extra = properties != null ? _jsonSerializer.Serialize(properties) : null;

			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					affected = await db.GetTable<DbFieldMeta>()
						.Where(x => x.Uid == item.Uid)
						.Set(x => x.TypeCode, item.Type)
						.Set(x => x.Key, item.Key)
						.Set(x => x.Name, item.Name)
						.Set(x => x.Description, item.Description)
						.Set(x => x.Placeholder, item.Placeholder)
						.Set(x => x.Icon, item.Icon)
						.Set(x => x.IsReadonly, item.Readonly)
						.Set(x => x.IsRequired, item.Required)
						.Set(x => x.DisplayOrder, item.DisplayOrder)
						.Set(x => x.Props, extra)
						.UpdateAsync(cancellationToken);
				}

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
