using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs;
using Montr.Docs.Services;
using Montr.Kompany.Registration.Commands;
using Montr.Kompany.Registration.Services.Implementations;

namespace Montr.Kompany.Registration.Services.CommandHandlers
{
	public class SubmitCompanyRegistrationRequestHandler : IRequestHandler<SubmitCompanyRegistrationRequest, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly CompanyRequestValidationHelper _validationHelper;
		private readonly IDocumentService _documentService;

		public SubmitCompanyRegistrationRequestHandler(IUnitOfWorkFactory unitOfWorkFactory,
			CompanyRequestValidationHelper validationHelper, IDocumentService documentService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_validationHelper = validationHelper;
			_documentService = documentService;
		}

		public async Task<ApiResult> Handle(SubmitCompanyRegistrationRequest request, CancellationToken cancellationToken)
		{
			var documentUid = request.DocumentUid ?? throw new ArgumentNullException(nameof(request.DocumentUid));

			await _validationHelper.EnsureCreatedByCurrentUser(documentUid, request.UserUid, cancellationToken);

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _documentService.ChangeStatus(documentUid, DocumentStatusCode.Submitted, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
