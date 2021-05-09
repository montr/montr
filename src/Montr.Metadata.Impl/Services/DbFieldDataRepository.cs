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

			await using (var db = _dbContextFactory.Create())
			{
				var fieldData = db.GetTable<DbFieldData>()
					// todo: load multiple items
					.Where(x => x.EntityTypeCode == request.EntityTypeCode && request.EntityUids.Contains(x.EntityUid))
					.AsEnumerable()
					.GroupBy(x => x.EntityUid)
					.ToList();

				var result = new SearchResult<FieldData>
				{
					Rows = fieldData.Select(x =>
					{
						var data = new FieldData();

						foreach (var dbData in x)
						{
							if (metadataMap.TryGetValue(dbData.Key, out var field))
							{
								var fieldProvider = _fieldProviderRegistry.GetFieldTypeProvider(field.Type);

								data[dbData.Key] = fieldProvider.ReadFromStorage(dbData.Value);
							}
						}

						return data;

					}).ToList()
				};

				return await Task.FromResult(result);
			}
		}

		public async Task<ApiResult> Validate(ManageFieldDataRequest request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Metadata));
			var metadata = request.Metadata ?? throw new ArgumentNullException(nameof(request.Metadata));

			var data = item.Fields ?? (item.Fields = new FieldData());

			var errors = new List<ApiResultError>();

			// todo: validate/insert/update system fields stored in FieldData
			foreach (var field in metadata /*.Where(x => x.System == false)*/)
			{
				data.TryGetValue(field.Key, out var value);

				var fieldProvider = _fieldProviderRegistry.GetFieldTypeProvider(field.Type);

				if (fieldProvider.Validate(value, out var parsed, out var fieldErrors))
				{
					data[field.Key] = parsed;
				}
				else
				{
					errors.Add(new ApiResultError
					{
						Key = FieldKey.FormatFullKey(field.Key),
						Messages = fieldErrors
					});
				}
			}

			var result = new ApiResult { Success = errors.Count == 0, Errors = errors };

			return await Task.FromResult(result);
		}

		public async Task<ApiResult> Insert(ManageFieldDataRequest request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Metadata));
			var metadata = request.Metadata ?? throw new ArgumentNullException(nameof(request.Metadata));

			var data = item.Fields ?? (item.Fields = new FieldData());

			var insertable = new List<DbFieldData>();

			// todo: validate/insert/update system fields stored in FieldData
			// todo: exclude db fields
			foreach (var field in metadata.Where(x => x.System == false))
			{
				var fieldProvider = _fieldProviderRegistry.GetFieldTypeProvider(field.Type);

				data.TryGetValue(field.Key, out var value);

				var storageValue = fieldProvider.WriteToStorage(value);

				insertable.Add(new DbFieldData
				{
					Uid = Guid.NewGuid(),
					EntityTypeCode = request.EntityTypeCode,
					EntityUid = request.EntityUid,
					Key = field.Key,
					Value = storageValue
				});
			}

			await using (var db = _dbContextFactory.Create())
			{
				var bc = await db.GetTable<DbFieldData>().BulkCopyAsync(insertable, cancellationToken);

				return new ApiResult { AffectedRows = bc.RowsCopied };
			}
		}

		public async Task<ApiResult> Update(ManageFieldDataRequest request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Metadata));
			var metadata = request.Metadata ?? throw new ArgumentNullException(nameof(request.Metadata));

			var data = item.Fields ?? (item.Fields = new FieldData());

			await using (var db = _dbContextFactory.Create())
			{
				// todo: reuse from context in request
				var existingData = await db.GetTable<DbFieldData>()
					.Where(x => x.EntityTypeCode == request.EntityTypeCode && x.EntityUid == request.EntityUid)
					.ToListAsync(cancellationToken);

				var existingMap = existingData.ToDictionary(x => x.Key);

				var insertable = new List<DbFieldData>();
				var updatable = new List<DbFieldData>();

				// todo: validate/insert/update system fields stored in FieldData
				// todo: exclude db fields
				foreach (var field in metadata.Where(x => x.System == false))
				{
					var fieldProvider = _fieldProviderRegistry.GetFieldTypeProvider(field.Type);

					data.TryGetValue(field.Key, out var value);

					var storageValue = fieldProvider.WriteToStorage(value);

					if (existingMap.TryGetValue(field.Key, out var dbField))
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
							Key = field.Key,
							Value = storageValue
						});
					}
				}

				// insert
				await db.GetTable<DbFieldData>().BulkCopyAsync(insertable, cancellationToken);

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

			return new ApiResult();
		}

		public async Task<ApiResult> Delete(DeleteFieldDataRequest request, CancellationToken cancellationToken)
		{
			await using (var db = _dbContextFactory.Create())
			{
				var affected = await db.GetTable<DbFieldData>()
					.Where(x => x.EntityTypeCode == request.EntityTypeCode && request.EntityUids.Contains(x.EntityUid))
					.DeleteAsync(cancellationToken);

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
