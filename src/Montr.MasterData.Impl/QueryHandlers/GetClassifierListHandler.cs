using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierListHandler : IRequestHandler<GetClassifierList, SearchResult<Classifier>>
	{
		private readonly IRepository<Classifier> _repository;

		public GetClassifierListHandler(IRepository<Classifier> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<Classifier>> Handle(GetClassifierList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
