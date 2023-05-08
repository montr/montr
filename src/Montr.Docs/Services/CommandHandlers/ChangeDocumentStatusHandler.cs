using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Commands;

namespace Montr.Docs.Services.CommandHandlers
{
	public class ChangeDocumentStatusHandler : IRequestHandler<ChangeDocumentStatus, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDocumentService _documentService;

		public ChangeDocumentStatusHandler(IUnitOfWorkFactory unitOfWorkFactory, IDocumentService documentService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_documentService = documentService;
		}
		
		public async Task<ApiResult> Handle(ChangeDocumentStatus request, CancellationToken cancellationToken)
		{
			var documentUid = request.DocumentUid ?? throw new ArgumentNullException(nameof(request.DocumentUid));

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _documentService.ChangeStatus(documentUid, request.StatusCode, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
