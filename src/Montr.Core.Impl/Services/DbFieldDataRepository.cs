using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

		public async Task Insert(string entityTypeCode, Guid entityUid, FieldData data, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				var fields = data.Select(x => new DbFieldData
				{
					Uid = Guid.NewGuid(),
					EntityTypeCode = entityTypeCode,
					EntityUid = entityUid,
					Key = x.Key,
					Value = x.Value,
				});

				await Task.Run(
					() => db.GetTable<DbFieldData>().BulkCopy(fields),
					cancellationToken);
			}
		}
	}
}
