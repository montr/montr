using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Docs.Queries;

namespace Montr.Docs.Services.QueryHandlers;

public class GetDocumentHandler : IRequestHandler<GetDocument, Document>
{
	private readonly IRepository<Document> _documentRepository;

	public GetDocumentHandler(IRepository<Document> documentRepository)
	{
		_documentRepository = documentRepository;
	}

	public async Task<Document> Handle(GetDocument request, CancellationToken cancellationToken)
	{
		var result = await _documentRepository.Search(new DocumentSearchRequest
		{
			Uid = request.Uid,
			IncludeFields = true
		}, cancellationToken);

		return result.Rows.Single();
	}
}