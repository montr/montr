using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Docs.Models;
using Montr.Docs.Queries;
using Montr.Docs.Services;

namespace Montr.Docs.Impl.QueryHandlers
{
	public class GetDocumentListHandler : IRequestHandler<GetDocumentList, SearchResult<Document>>
	{
		private readonly IDocumentRepository _repository;

		public GetDocumentListHandler(IDocumentRepository repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<Document>> Handle(GetDocumentList request, CancellationToken cancellationToken)
		{
			return await _repository.Search(request, cancellationToken);
		}
	}
}
