using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Commands;
using Montr.Docs.Services;
using Montr.Kompany.Registration.Commands;
using Montr.Kompany.Registration.Services.Implementations;

namespace Montr.Kompany.Registration.Services.CommandHandlers
{
	public class DeleteCompanyRegistrationRequestHandler : IRequestHandler<DeleteCompanyRegistrationRequest, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly CompanyRequestValidationHelper _validationHelper;
		private readonly IDocumentService _documentService;

		public DeleteCompanyRegistrationRequestHandler(IUnitOfWorkFactory unitOfWorkFactory,
			CompanyRequestValidationHelper validationHelper, IDocumentService documentService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_validationHelper = validationHelper;
			_documentService = documentService;
		}

		public async Task<ApiResult> Handle(DeleteCompanyRegistrationRequest request, CancellationToken cancellationToken)
		{
			await _validationHelper.EnsureCreatedByCurrentUser(request.DocumentUid, request.UserUid, cancellationToken);

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _documentService.Delete(new DeleteDocument
				{
					UserUid = request.UserUid,
					Uids = new[] { request.DocumentUid }
				}, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
