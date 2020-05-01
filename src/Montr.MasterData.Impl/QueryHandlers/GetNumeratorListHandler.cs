using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetNumeratorListHandler : IRequestHandler<GetNumeratorList, SearchResult<Numerator>>
	{
		private readonly IRepository<Numerator> _repository;

		public GetNumeratorListHandler(IRepository<Numerator> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<Numerator>> Handle(GetNumeratorList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
