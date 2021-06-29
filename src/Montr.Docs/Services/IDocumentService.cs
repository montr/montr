using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Docs.Commands;
using Montr.Docs.Models;

namespace Montr.Docs.Services
{
	public interface IDocumentService
	{
		Task<SearchResult<Document>> Search(DocumentSearchRequest request, CancellationToken cancellationToken);

		Task<ApiResult> Create(Document document, CancellationToken cancellationToken);

		Task<ApiResult> Delete(DeleteDocument request, CancellationToken cancellationToken);
	}
}
