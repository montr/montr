using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierListHandler : IRequestHandler<GetClassifierList, SearchResult<Classifier>>
	{
		private readonly IClassifierRepository _repository;

		public GetClassifierListHandler(IClassifierRepository repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<Classifier>> Handle(GetClassifierList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
