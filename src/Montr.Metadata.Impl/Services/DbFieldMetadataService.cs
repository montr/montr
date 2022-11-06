using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB.Data;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Metadata.Impl.Entities;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Metadata.Impl.Services
{
	public class DbFieldMetadataService : IFieldMetadataService
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IJsonSerializer _jsonSerializer;

		public DbFieldMetadataService(IDbContextFactory dbContextFactory, IDateTimeProvider dateTimeProvider, IJsonSerializer jsonSerializer)
		{
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
			_jsonSerializer = jsonSerializer;
		}

		public async Task<ApiResult> Insert(ManageFieldMetadataRequest request, CancellationToken cancellationToken)
		{
			var items = request.Items ?? throw new ArgumentNullException(nameof(request.Items));

			var dbItems = new List<DbFieldMetadata>();

			foreach (var item in items)
			{
				var properties = item.GetProperties();

				var extra = properties != null ? _jsonSerializer.Serialize(properties) : null;

				dbItems.Add(new DbFieldMetadata
				{
					Uid = Guid.NewGuid(),
					EntityTypeCode = request.EntityTypeCode,
					EntityUid = request.EntityUid,
					Type = item.Type,
					Key = item.Key,
					Name = item.Name,
					Description = item.Description,
					Placeholder = item.Placeholder,
					Icon = item.Icon,
					IsActive = true,
					IsSystem = item.System,
					IsReadonly = item.Readonly,
					IsRequired = item.Required,
					DisplayOrder = item.DisplayOrder,
					CreatedAtUtc = _dateTimeProvider.GetUtcNow(),
					Props = extra
				});
			}

			using (var db = _dbContextFactory.Create())
			{
				var rowsCopied = await db.GetTable<DbFieldMetadata>()
					.BulkCopyAsync(dbItems, cancellationToken);

				return new ApiResult
				{
					AffectedRows = rowsCopied.RowsCopied,
					Uid = dbItems.Count == 1 ? dbItems[0].Uid : null
				};
			}
		}
	}
}
