using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Automate.Models;
using Montr.Automate.Queries;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Automate.Impl.QueryHandlers
{
	public class GetAutomationListHandler : IRequestHandler<GetAutomationList, SearchResult<Automation>>
	{
		private readonly IRepository<Automation> _repository;

		public GetAutomationListHandler(IRepository<Automation> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<Automation>> Handle(GetAutomationList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
