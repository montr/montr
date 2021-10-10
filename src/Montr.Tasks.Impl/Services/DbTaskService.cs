using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Data.Linq2Db;
using Montr.Tasks.Impl.Entities;
using Montr.Tasks.Models;
using Montr.Tasks.Services;

namespace Montr.Tasks.Impl.Services
{
	public class DbTaskService : ITaskService
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbTaskService(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Insert(TaskModel item, CancellationToken cancellationToken)
		{
			var itemUid = Guid.NewGuid();

			// todo: validation and limits

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbTask>()
					.Value(x => x.Uid, itemUid)
					.Value(x => x.Name, item.Name)
					.InsertAsync(cancellationToken);
			}

			return new ApiResult { Uid = itemUid };
		}

		public async Task<ApiResult> Update(TaskModel item, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbTask>()
					.Where(x => x.Uid == item.Uid)
					.Set(x => x.Name, item.Name)
					.UpdateAsync(cancellationToken);

				return new ApiResult();
			}
		}

		public async Task<ApiResult> Delete(Guid[] uids, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				var affected = await db.GetTable<DbTask>()
					.Where(x => uids.Contains(x.Uid))
					.DeleteAsync(cancellationToken);

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
