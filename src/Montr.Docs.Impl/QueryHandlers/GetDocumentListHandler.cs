using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Docs.Queries;

namespace Montr.Docs.Impl.QueryHandlers
{
	public class GetDocumentListHandler : IRequestHandler<GetDocumentList, SearchResult<Document>>
	{
		private readonly IRepository<Document> _repository;

		public GetDocumentListHandler(IRepository<Document> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<Document>> Handle(GetDocumentList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
