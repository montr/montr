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

				var paged = all.Apply(request, x => x.Code, SortOrder.Descending);

				if (request.Uid != null)
				{
					all = all.Where(x => x.Uid == request.Uid);
				}

				var data = await paged
					.Select(x => new TaskModel
					{
						Uid = x.Uid,
						StatusCode = x.StatusCode,
						Code = x.Code,
						Name = x.Name,
						Url = $"/tasks/{x.Code}/"
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<TaskModel>
				{
					TotalCount = all.GetTotalCount(request),
					Rows = data
				};
			}
		}
	}
}
