using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetNumeratorEntityListHandler : IRequestHandler<GetNumeratorEntityList, SearchResult<NumeratorEntity>>
	{
		private readonly IRepository<NumeratorEntity> _repository;

		public GetNumeratorEntityListHandler(IRepository<NumeratorEntity> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<NumeratorEntity>> Handle(GetNumeratorEntityList searchRequest, CancellationToken cancellationToken)
		{
			return await _repository.Search(searchRequest, cancellationToken);
		}
	}
}
