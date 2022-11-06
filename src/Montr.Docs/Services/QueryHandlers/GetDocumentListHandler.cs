using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Docs.Queries;

namespace Montr.Docs.Services.QueryHandlers;

public class GetDocumentListHandler : IRequestHandler<GetDocumentList, SearchResult<Document>>
{
	private readonly IRepository<Document> _documentRepository;

	public GetDocumentListHandler(IRepository<Document> documentRepository)
	{
		_documentRepository = documentRepository;
	}

	public async Task<SearchResult<Document>> Handle(GetDocumentList request, CancellationToken cancellationToken)
	{
		return await _documentRepository.Search(request, cancellationToken);
	}
}