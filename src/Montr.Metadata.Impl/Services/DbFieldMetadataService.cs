using System;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Metadata.Impl.Entities;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Metadata.Impl.Services
{
	public class DbFieldMetadataService : IFieldMetadataService
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IJsonSerializer _jsonSerializer;

		public DbFieldMetadataService(IDbContextFactory dbContextFactory, IJsonSerializer jsonSerializer)
		{
			_dbContextFactory = dbContextFactory;
			_jsonSerializer = jsonSerializer;
		}

		public async Task<ApiResult> Insert(ManageFieldMetadataRequest request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var properties = item.GetProperties();

			var extra = properties != null ? _jsonSerializer.Serialize(properties) : null;

			var itemUid = Guid.NewGuid();

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbFieldMetadata>()
					.Value(x => x.Uid, itemUid)
					.Value(x => x.EntityTypeCode, request.EntityTypeCode)
					.Value(x => x.EntityUid, request.EntityUid)
					.Value(x => x.Type, item.Type)
					.Value(x => x.Key, item.Key)
					.Value(x => x.Name, item.Name)
					.Value(x => x.Description, item.Description)
					.Value(x => x.Placeholder, item.Placeholder)
					.Value(x => x.Icon, item.Icon)
					.Value(x => x.IsActive, true)
					.Value(x => x.IsSystem, item.System)
					.Value(x => x.IsReadonly, item.Readonly)
					.Value(x => x.IsRequired, item.Required)
					.Value(x => x.DisplayOrder, item.DisplayOrder)
					.Value(x => x.Props, extra)
					.InsertAsync(cancellationToken);

				return new ApiResult { Uid = itemUid };
			}
		}
	}
}
