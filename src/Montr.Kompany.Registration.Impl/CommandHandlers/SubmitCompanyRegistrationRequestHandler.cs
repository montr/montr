using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs;
using Montr.Docs.Services;
using Montr.Kompany.Registration.Commands;
using Montr.Kompany.Registration.Impl.Services;

namespace Montr.Kompany.Registration.Impl.CommandHandlers
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
			await _validationHelper.EnsureCreatedByCurrentUser(request.DocumentUid, request.UserUid, cancellationToken);

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _documentService.ChangeStatus(request.DocumentUid, DocumentStatusCode.Submitted, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
