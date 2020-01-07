using System;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Metadata.Commands;
using Montr.Metadata.Impl.Entities;

namespace Montr.Metadata.Impl.CommandHandlers
{
	public class InsertDataFieldHandler : IRequestHandler<InsertDataField, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IJsonSerializer _jsonSerializer;

		public InsertDataFieldHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory, IJsonSerializer jsonSerializer)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_jsonSerializer = jsonSerializer;
		}

		public async Task<ApiResult> Handle(InsertDataField request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var properties = item.GetProperties();

			var extra = properties != null ? _jsonSerializer.Serialize(properties) : null;

			using (var scope = _unitOfWorkFactory.Create())
			{
				var itemUid = Guid.NewGuid();

				using (var db = _dbContextFactory.Create())
				{
					await db.GetTable<DbFieldMetadata>()
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
						.Value(x => x.IsReadonly, item.Readonly)
						.Value(x => x.IsRequired, item.Required)
						.Value(x => x.DisplayOrder, item.DisplayOrder)
						.Value(x => x.Props, extra)
						.InsertAsync(cancellationToken);

					scope.Commit();
				}

				return new ApiResult { Uid = itemUid };
			}
		}
	}
}
