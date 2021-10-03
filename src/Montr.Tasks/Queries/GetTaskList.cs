using MediatR;
using Montr.Core.Models;
using Montr.Tasks.Models;

namespace Montr.Tasks.Queries
{
	public class GetTaskList : TaskSearchRequest, IRequest<SearchResult<TaskModel>>
	{
	}
}
