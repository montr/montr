using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Tasks.Models;
using Montr.Tasks.Queries;

namespace Montr.Tasks.Services.QueryHandlers
{
	public class GetTaskListHandler : IRequestHandler<GetTaskList, SearchResult<TaskModel>>
	{
		private readonly IRepository<TaskModel> _repository;

		public GetTaskListHandler(IRepository<TaskModel> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<TaskModel>> Handle(GetTaskList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
