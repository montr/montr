using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Montr.Core.Impl.Entities;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.Services
{
	public class DbFieldDataRepository : IFieldDataRepository
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbFieldDataRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<FieldData>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (FieldDataSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

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
							data[dbData.Key] = dbData.Value;
						}

						return data;

					}).ToList()
				};
			}
		}

		public async Task<ApiResult> Insert(FieldDataRequest request, CancellationToken cancellationToken)
		{
			var data = request.Data ?? throw new ArgumentNullException(nameof(request.Data));
			var metadata = request.Metadata ?? throw new ArgumentNullException(nameof(request.Metadata));

			var metadataKeys = new HashSet<string>(metadata.Select(x => x.Key));
			var workingData = data.Where(x => metadataKeys.Contains(x.Key)).ToList();

			var fields = workingData.Select(x => new DbFieldData
			{
				Uid = Guid.NewGuid(),
				EntityTypeCode = request.EntityTypeCode,
				EntityUid = request.EntityUid,
				Key = x.Key,
				Value = x.Value,
			});

			using (var db = _dbContextFactory.Create())
			{
				var bcr = await Task.Run(() => db.GetTable<DbFieldData>().BulkCopy(fields), cancellationToken);

				return new ApiResult { AffectedRows = bcr.RowsCopied };
			}
		}

		public async Task<ApiResult> Update(FieldDataRequest request, CancellationToken cancellationToken)
		{
			var data = request.Data ?? throw new ArgumentNullException(nameof(request.Data));
			var metadata = request.Metadata ?? throw new ArgumentNullException(nameof(request.Metadata));

			var metadataKeys = new HashSet<string>(metadata.Select(x => x.Key));
			var workingData = data.Where(x => metadataKeys.Contains(x.Key)).ToList();

			using (var db = _dbContextFactory.Create())
			{
				// todo: reuse from context in request
				var existingData = await db.GetTable<DbFieldData>()
					.Where(x => x.EntityTypeCode == request.EntityTypeCode && x.EntityUid == request.EntityUid)
					.ToListAsync(cancellationToken);

				var existingKeys = new HashSet<string>(existingData.Select(x => x.Key));

				// insert
				var insertable = workingData
					.Where(x => existingKeys.Contains(x.Key) == false)
					.Select(x => new DbFieldData
					{
						Uid = Guid.NewGuid(),
						EntityTypeCode = request.EntityTypeCode,
						EntityUid = request.EntityUid,
						Key = x.Key,
						Value = x.Value,
					});

				db.GetTable<DbFieldData>().BulkCopy(insertable);

				// update
				var updatable = (
					from working in workingData
					join existing in existingData on working.Key equals existing.Key
					select new DbFieldData
					{
						Uid = existing.Uid,
						Key = existing.Key,
						Value = working.Value
					}).ToList();

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
