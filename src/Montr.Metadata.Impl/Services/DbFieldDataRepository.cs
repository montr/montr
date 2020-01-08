using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Montr.Core.Models;
using Montr.Data.Linq2Db;
using Montr.Metadata.Impl.Entities;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Metadata.Impl.Services
{
	public class DbFieldDataRepository : IFieldDataRepository
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IFieldProviderRegistry _fieldProviderRegistry;

		public DbFieldDataRepository(IDbContextFactory dbContextFactory, IFieldProviderRegistry fieldProviderRegistry)
		{
			_dbContextFactory = dbContextFactory;
			_fieldProviderRegistry = fieldProviderRegistry;
		}

		public async Task<SearchResult<FieldData>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (FieldDataSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));
			var metadata = request.Metadata ?? throw new ArgumentNullException(nameof(request.Metadata));

			var metadataMap = metadata.ToDictionary(x => x.Key);

			using (var db = _dbContextFactory.Create())
			{
				var result = await db.GetTable<DbFieldData>()
					// todo: load multiple items
					.Where(x => x.EntityTypeCode == request.EntityTypeCode && request.EntityUids.Contains(x.EntityUid))
					.GroupBy(x => x.EntityUid)
					.Select(x => x)
					.ToListAsync(cancellationToken);

				return new SearchResult<FieldData>
				{
					Rows = result.Select(x =>
					{
						var data = new FieldData();

						foreach (var dbData in x)
						{
							if (metadataMap.TryGetValue(dbData.Key, out var field))
							{
								var fieldProvider = _fieldProviderRegistry.GetFieldTypeProvider(field.Type);

								data[dbData.Key] = fieldProvider.Read(dbData.Value);
							}
						}

						return data;

					}).ToList()
				};
			}
		}

		public async Task<ApiResult> Insert(ManageFieldDataRequest request, CancellationToken cancellationToken)
		{
			var metadata = request.Metadata ?? throw new ArgumentNullException(nameof(request.Metadata));

			if (request.Data != null)
			{
				var metadataMap = metadata.ToDictionary(x => x.Key);

				var fields = new List<DbFieldData>();

				foreach (var data in request.Data)
				{
					if (metadataMap.TryGetValue(data.Key, out var field))
					{
						var fieldProvider = _fieldProviderRegistry.GetFieldTypeProvider(field.Type);

						fields.Add(new DbFieldData
						{
							Uid = Guid.NewGuid(),
							EntityTypeCode = request.EntityTypeCode,
							EntityUid = request.EntityUid,
							Key = data.Key,
							Value = fieldProvider.Write(data.Value)
					});
					}
				}

				using (var db = _dbContextFactory.Create())
				{
					var bcr = await Task.Run(() => db.GetTable<DbFieldData>().BulkCopy(fields), cancellationToken);

					return new ApiResult { AffectedRows = bcr.RowsCopied };
				}
			}

			return new ApiResult { AffectedRows = 0 };
		}

		public async Task<ApiResult> Update(ManageFieldDataRequest request, CancellationToken cancellationToken)
		{
			var metadata = request.Metadata ?? throw new ArgumentNullException(nameof(request.Metadata));

			if (request.Data != null)
			{
				var metadataMap = metadata.ToDictionary(x => x.Key);

				using (var db = _dbContextFactory.Create())
				{
					// todo: reuse from context in request
					var existingData = await db.GetTable<DbFieldData>()
						.Where(x => x.EntityTypeCode == request.EntityTypeCode && x.EntityUid == request.EntityUid)
						.ToListAsync(cancellationToken);

					var existingMap = existingData.ToDictionary(x => x.Key);

					var insertable = new List<DbFieldData>();
					var updatable = new List<DbFieldData>();

					foreach (var (key, value) in request.Data)
					{
						if (metadataMap.TryGetValue(key, out var field))
						{
							var fieldProvider = _fieldProviderRegistry.GetFieldTypeProvider(field.Type);
							var storageValue = fieldProvider.Write(value);

							if (existingMap.TryGetValue(key, out var dbField))
							{
								updatable.Add(new DbFieldData
								{
									Uid = dbField.Uid,
									Key = dbField.Key,
									Value = storageValue
								});
							}
							else
							{
								insertable.Add(new DbFieldData
								{
									Uid = Guid.NewGuid(),
									EntityTypeCode = request.EntityTypeCode,
									EntityUid = request.EntityUid,
									Key = key,
									Value = storageValue,
								});
							}
						}
					}

					// insert
					db.GetTable<DbFieldData>().BulkCopy(insertable);

					// update
					foreach (var field in updatable)
					{
						await db.GetTable<DbFieldData>()
							.Where(x => x.Uid == field.Uid)
							.Set(x => x.Value, field.Value)
							.UpdateAsync(cancellationToken);
					}

					// delete (-)? or leave non-active data in db (+)?
					// not used keys should be deleted when corresponding metadata deleted
				}
			}

			return new ApiResult();
		}

		public async Task<ApiResult> Delete(DeleteFieldDataRequest request, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				var affected = await db.GetTable<DbFieldData>()
					.Where(x => x.EntityTypeCode == request.EntityTypeCode && request.EntityUids.Contains(x.Uid))
					.DeleteAsync(cancellationToken);

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
