using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Docs.Services;
using Montr.Kompany.Models;
using Montr.Kompany.Registration.Commands;
using Montr.MasterData.Services;

namespace Montr.Kompany.Registration.Impl.CommandHandlers
{
	public class CreateCompanyRegistrationRequestHandler : IRequestHandler<CreateCompanyRegistrationRequest, ApiResult>
	{
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;
		private readonly IDocumentService _documentService;

		public CreateCompanyRegistrationRequestHandler(IDateTimeProvider dateTimeProvider,
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory, IDocumentService documentService)
		{
			_dateTimeProvider = dateTimeProvider;
			_classifierRepositoryFactory = classifierRepositoryFactory;
			_documentService = documentService;
		}

		public async Task<ApiResult> Handle(CreateCompanyRegistrationRequest request, CancellationToken cancellationToken)
		{
			var documentTypeRepository = _classifierRepositoryFactory.GetNamedOrDefaultService(Docs.ClassifierTypeCode.DocumentType);
			var documentType = await documentTypeRepository.Get(Docs.ClassifierTypeCode.DocumentType, DocumentTypes.CompanyRegistrationRequest, cancellationToken);

			var now = _dateTimeProvider.GetUtcNow();

			// todo: fill company uid if company already exists
			var document = new Document
			{
				// ReSharper disable once PossibleInvalidOperationException
				DocumentTypeUid = documentType.Uid.Value,
				DocumentDate = now,
				Direction = DocumentDirection.Outgoing,
				CreatedAtUtc = now,
				CreatedBy = request.UserUid
			};

			var result = await _documentService.Create(document, cancellationToken);

			return result;
		}
	}
}
