using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Kompany.Models;
using Montr.Kompany.Queries;

namespace Montr.Kompany.Impl.QueryHandlers
{
	public class GetCompanyListHandler : IRequestHandler<GetCompanyList, SearchResult<Company>>
	{
		private readonly IRepository<Company> _repository;

		public GetCompanyListHandler(IRepository<Company> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<Company>> Handle(GetCompanyList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
