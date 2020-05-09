using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Docs.Models;

namespace Montr.Docs.Services
{
	public interface IDocumentTypeService
	{
		Task<DocumentType> TryGet(string typeCode, CancellationToken cancellationToken);

		Task<DocumentType> Get(string typeCode, CancellationToken cancellationToken);

		Task<DocumentType> TryGet(Guid uid, CancellationToken cancellationToken);

		Task<DocumentType> Get(Guid uid, CancellationToken cancellationToken);

		Task<ApiResult> Insert(DocumentType item, CancellationToken cancellationToken);}
}
