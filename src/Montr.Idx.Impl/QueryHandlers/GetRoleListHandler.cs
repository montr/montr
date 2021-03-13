using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Models;
using Montr.Idx.Queries;

namespace Montr.Idx.Impl.QueryHandlers
{
	public class GetRoleListHandler : IRequestHandler<GetRoleList, SearchResult<Role>>
	{
		private readonly IRepository<Role> _repository;

		public GetRoleListHandler(IRepository<Role> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<Role>> Handle(GetRoleList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
