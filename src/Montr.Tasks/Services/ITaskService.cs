using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Tasks.Models;

namespace Montr.Tasks.Services
{
	public interface ITaskService
	{
		Task<ApiResult> Insert(TaskModel item, CancellationToken cancellationToken);

		Task<ApiResult> Update(TaskModel item, CancellationToken cancellationToken);

		Task<ApiResult> Delete(Guid[] uids, CancellationToken cancellationToken);
	}
}
