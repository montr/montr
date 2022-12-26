using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Docs.Commands;
using Montr.Docs.Models;

namespace Montr.Docs.Services
{
	public interface IDocumentService
	{
		Task<ApiResult> Create(Document document, CancellationToken cancellationToken = default);

		Task<ApiResult> ChangeStatus(Guid documentUid, string statusCode, CancellationToken cancellationToken = default);

		Task<ApiResult> Delete(DeleteDocument request, CancellationToken cancellationToken = default);
	}
}