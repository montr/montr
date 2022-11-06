using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Services.QueryHandlers
{
	public class GetClassifierListHandler : IRequestHandler<GetClassifierList, SearchResult<Classifier>>
	{
		private readonly INamedServiceFactory<IClassifierRepository> _repositoryFactory;

		public GetClassifierListHandler(INamedServiceFactory<IClassifierRepository> repositoryFactory)
		{
			_repositoryFactory = repositoryFactory;
		}

		public async Task<SearchResult<Classifier>> Handle(GetClassifierList request, CancellationToken cancellationToken)
		{
			var repository = _repositoryFactory.GetNamedOrDefaultService(request.TypeCode);

			return await repository.Search(request, cancellationToken);
		}
	}
}
