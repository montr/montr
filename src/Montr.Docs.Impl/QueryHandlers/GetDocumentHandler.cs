using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Docs.Queries;

namespace Montr.Docs.Impl.QueryHandlers
{
	public class GetDocumentHandler : IRequestHandler<GetDocument, Document>
	{
		private readonly IRepository<Document> _repository;

		public GetDocumentHandler(IRepository<Document> repository)
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
