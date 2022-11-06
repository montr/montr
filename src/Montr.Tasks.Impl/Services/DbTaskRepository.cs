using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Entities;
using Montr.Tasks.Impl.Entities;
using Montr.Tasks.Models;

namespace Montr.Tasks.Impl.Services
{
	public class DbTaskRepository : IRepository<TaskModel>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbTaskRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<TaskModel>> Search(SearchRequest searchRequest, CancellationToken cancellationToken = default)
		{
			var request = (TaskSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var all = db.GetTable<DbTask>().AsQueryable();

				if (request.Uid != null)
				{
					all = all.Where(x => x.Uid == request.Uid);
				}

				var dbTaskTypes = db.GetTable<DbClassifier>();

				var query = from task in all
					from taskType in dbTaskTypes.LeftJoin(taskType => task.TaskTypeUid == taskType.Uid)
					select new DbItem
					{
						Task = task,
						TaskType = taskType
					};

				// todo: fix paging - map column to expression
				request.SortColumn ??= nameof(TaskModel.CreatedAtUtc);
				request.SortColumn = nameof(DbItem.Task) + "." + request.SortColumn;

				var paged = query.Apply(request, x => x.Task.CreatedAtUtc, SortOrder.Descending);

				var data = await paged
					.Select(x => new TaskModel
					{
						Uid = x.Task.Uid,
						StatusCode = x.Task.StatusCode,
						TaskTypeUid = x.Task.TaskTypeUid,
						TaskTypeName = x.TaskType.Name,
						Number = x.Task.Number,
						Name = x.Task.Name,
						Description = x.Task.Description,
						AssigneeUid = x.Task.AssigneeUid,
						StartDateUtc = x.Task.StartDateUtc,
						DueDateUtc = x.Task.DueDateUtc,
						CreatedAtUtc = x.Task.CreatedAtUtc,
						ModifiedAtUtc = x.Task.ModifiedAtUtc,
						Url = $"/tasks/view/{x.Task.Uid}/"
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<TaskModel>
				{
					TotalCount = all.GetTotalCount(request),
					Rows = data
				};
			}
		}

		private class DbItem
		{
			public DbTask Task { get; init; }

			public DbClassifier TaskType { get; init; }
		}
	}
}
