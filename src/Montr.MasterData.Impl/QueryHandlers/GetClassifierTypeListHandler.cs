using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierTypeListHandler : IRequestHandler<GetClassifierTypeList, SearchResult<ClassifierType>>
	{
		private readonly IRepository<ClassifierType> _repository;

		public GetClassifierTypeListHandler(IRepository<ClassifierType> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<ClassifierType>> Handle(GetClassifierTypeList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
