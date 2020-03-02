using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Docs.Models;
using Montr.Docs.Queries;
using Montr.Docs.Services;

namespace Montr.Docs.Impl.QueryHandlers
{
	public class GetDocumentHandler : IRequestHandler<GetDocument, Document>
	{
		private readonly IDocumentRepository _repository;

		public GetDocumentHandler(IDocumentRepository repository)
		{
			_repository = repository;
		}

		public async Task<Document> Handle(GetDocument request, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new DocumentSearchRequest
			{
				Uid = request.Uid,
				IncludeFields = true
			}, cancellationToken);

			return result.Rows.Single();
		}
	}
}
