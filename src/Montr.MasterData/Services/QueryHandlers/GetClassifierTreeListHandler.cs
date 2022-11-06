using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Services.QueryHandlers
{
	public class GetClassifierTreeListHandler : IRequestHandler<GetClassifierTreeList, SearchResult<ClassifierTree>>
	{
		private readonly IRepository<ClassifierTree> _repository;

		public GetClassifierTreeListHandler(IRepository<ClassifierTree> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<ClassifierTree>> Handle(GetClassifierTreeList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
