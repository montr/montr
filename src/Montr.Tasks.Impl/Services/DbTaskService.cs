using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Tasks.Impl.Entities;
using Montr.Tasks.Models;
using Montr.Tasks.Services;

namespace Montr.Tasks.Impl.Services
{
	public class DbTaskService : ITaskService
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;

		public DbTaskService(IDbContextFactory dbContextFactory, IDateTimeProvider dateTimeProvider)
		{
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
		}

		public async Task<ApiResult> Insert(TaskModel item, CancellationToken cancellationToken)
		{
			var itemUid = Guid.NewGuid();

			var now = _dateTimeProvider.GetUtcNow();

			// todo: validation and limits

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbTask>()
					.Value(x => x.Uid, itemUid)
					.Value(x => x.StatusCode, TaskStatusCode.Open)
					.Value(x => x.CompanyUid, item.CompanyUid)
					.Value(x => x.TaskTypeUid, item.TaskTypeUid)
					.Value(x => x.AssigneeUid, item.AssigneeUid)
					.Value(x => x.Number, item.Number)
					.Value(x => x.Name, item.Name)
					.Value(x => x.Description, item.Description)
					.Value(x => x.CreatedAtUtc, now)
					.InsertAsync(cancellationToken);
			}

			return new ApiResult { Uid = itemUid };
		}

		public async Task<ApiResult> Update(TaskModel item, CancellationToken cancellationToken)
		{
			var now = _dateTimeProvider.GetUtcNow();

			using (var db = _dbContextFactory.Create())
			{
				// todo: check company uid
				var affected = await db.GetTable<DbTask>()
					.Where(x => x.Uid == item.Uid)
					.Set(x => x.TaskTypeUid, item.TaskTypeUid)
					.Set(x => x.AssigneeUid, item.AssigneeUid)
					.Set(x => x.Number, item.Number)
					.Set(x => x.Name, item.Name)
					.Set(x => x.Description, item.Description)
					.Set(x => x.ModifiedAtUtc, now)
					.UpdateAsync(cancellationToken);

				return new ApiResult { AffectedRows = affected };
			}
		}

		public async Task<ApiResult> Delete(Guid[] uids, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				// todo: check company uid
				var affected = await db.GetTable<DbTask>()
					.Where(x => uids.Contains(x.Uid))
					.DeleteAsync(cancellationToken);

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
