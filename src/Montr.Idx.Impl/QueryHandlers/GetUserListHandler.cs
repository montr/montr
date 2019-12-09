using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Models;
using Montr.Idx.Queries;

namespace Montr.Idx.Impl.QueryHandlers
{
	public class GetUserListHandler : IRequestHandler<GetUserList, SearchResult<User>>
	{
		private readonly IRepository<User> _repository;

		public GetUserListHandler(IRepository<User> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<User>> Handle(GetUserList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
