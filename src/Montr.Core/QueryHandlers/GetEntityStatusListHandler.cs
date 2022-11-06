using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;

namespace Montr.Core.QueryHandlers
{
	public class GetEntityStatusListHandler : IRequestHandler<GetEntityStatusList, SearchResult<EntityStatus>>
	{
		private readonly IRepository<EntityStatus> _repository;

		public GetEntityStatusListHandler(IRepository<EntityStatus> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<EntityStatus>> Handle(GetEntityStatusList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
