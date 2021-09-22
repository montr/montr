using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Commands;
using Montr.Docs.Services;

namespace Montr.Docs.Impl.CommandHandlers
{
	public class SubmitDocumentHandler : IRequestHandler<SubmitDocument, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDocumentService _documentService;

		public SubmitDocumentHandler(IUnitOfWorkFactory unitOfWorkFactory, IDocumentService documentService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_documentService = documentService;
		}

		public async Task<ApiResult> Handle(SubmitDocument request, CancellationToken cancellationToken)
		{
			var documentUid = request.DocumentUid ?? throw new ArgumentNullException(nameof(request.DocumentUid));

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _documentService.ChangeStatus(documentUid, DocumentStatusCode.Submitted, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
