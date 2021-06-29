using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Commands;
using Montr.Docs.Services;
using Montr.Kompany.Registration.Commands;

namespace Montr.Kompany.Registration.Impl.CommandHandlers
{
	public class DeleteCompanyRegistrationRequestHandler : IRequestHandler<DeleteCompanyRegistrationRequest, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDocumentService _documentService;

		public DeleteCompanyRegistrationRequestHandler(
			IUnitOfWorkFactory unitOfWorkFactory,
			IDocumentService documentService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_documentService = documentService;
		}

		public async Task<ApiResult> Handle(DeleteCompanyRegistrationRequest request, CancellationToken cancellationToken)
		{
			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _documentService.Delete(new DeleteDocument
				{
					UserUid = request.UserUid,
					Uids = new[] { request.Uid }
				}, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
