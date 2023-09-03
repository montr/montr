using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Commands;
using Montr.Docs.Models;
using Montr.MasterData.Services;

namespace Montr.Docs.Services.CommandHandlers
{
	public class CreateRelatedDocumentHandler : IRequestHandler<CreateRelatedDocument, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IConfigurationProvider _configurationProvider;
		private readonly IRepository<Document> _documentRepository;
		private readonly IDocumentService _documentService;
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;
		private readonly IEntityRelationService _entityRelationService;

		public CreateRelatedDocumentHandler(IUnitOfWorkFactory unitOfWorkFactory, IDateTimeProvider dateTimeProvider,
			IConfigurationProvider configurationProvider,
			IRepository<Document> documentRepository, IDocumentService documentService,
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory,
			IEntityRelationService entityRelationService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dateTimeProvider = dateTimeProvider;
			_configurationProvider = configurationProvider;
			_documentRepository = documentRepository;
			_documentService = documentService;
			_classifierRepositoryFactory = classifierRepositoryFactory;
			_entityRelationService = entityRelationService;
		}

		public async Task<ApiResult> Handle(CreateRelatedDocument request, CancellationToken cancellationToken)
		{
			var documentUid = request.DocumentUid ?? throw new ArgumentNullException(nameof(request.DocumentUid));

			var searchRequest = new DocumentSearchRequest { Uid = documentUid };
			var searchResult = await _documentRepository.Search(searchRequest, cancellationToken);

			var document = searchResult.Rows.Single();

			var items = await _configurationProvider.GetItems<Document, CreateRelatedDocument>(document, request.Principal);
			var config = items.Single();

			var documentTypeRepository = _classifierRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.DocumentType);
			var documentType = await documentTypeRepository.Get(ClassifierTypeCode.DocumentType, config.DocumentTypeCode, cancellationToken);

			if (documentType == null)
			{
				return new ApiResult
				{
					Success = false,
					Errors = new[]
					{
						new ApiResultError
						{
							Messages = new[] { "Document type " + config.DocumentTypeCode + " is not found" }
						}
					}
				};
			}

			var now = _dateTimeProvider.GetUtcNow();

			var related = new Document
			{
				DocumentTypeUid = documentType.Uid!.Value,
				DocumentDate = now,
				Direction = DocumentDirection.Internal,
				CreatedAtUtc = now,
				CreatedBy = request.UserUid
			};

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _documentService.Create(related, cancellationToken);

				if (result.Success)
				{
					var relation = new EntityRelation
					{
						EntityTypeCode = EntityTypeCode.Document,
						EntityUid = document.Uid!.Value,
						RelatedEntityTypeCode = EntityTypeCode.Document,
						RelatedEntityUid = related.Uid!.Value,
						RelationType = config.RelationTypeCode
					};

					result = await _entityRelationService.Insert(relation, cancellationToken);
				}
				
				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
