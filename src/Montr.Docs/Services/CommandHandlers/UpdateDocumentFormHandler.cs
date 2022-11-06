using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Commands;
using Montr.Docs.Models;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Docs.Services.CommandHandlers;

public class UpdateDocumentFormHandler : IRequestHandler<UpdateDocumentForm, ApiResult>
{
	private readonly IUnitOfWorkFactory _unitOfWorkFactory;
	private readonly IRepository<Document> _documentRepository;
	private readonly IRepository<FieldMetadata> _fieldMetadataRepository;
	private readonly IFieldDataRepository _fieldDataRepository;

	public UpdateDocumentFormHandler(
		IUnitOfWorkFactory unitOfWorkFactory,
		IRepository<Document> documentRepository,
		IRepository<FieldMetadata> fieldMetadataRepository,
		IFieldDataRepository fieldDataRepository)
	{
		_unitOfWorkFactory = unitOfWorkFactory;
		_documentRepository = documentRepository;
		_fieldMetadataRepository = fieldMetadataRepository;
		_fieldDataRepository = fieldDataRepository;
	}

	public async Task<ApiResult> Handle(UpdateDocumentForm request, CancellationToken cancellationToken)
	{
		var documentSearchRequest = new DocumentSearchRequest { Uid = request.DocumentUid };

		var document = (await _documentRepository.Search(documentSearchRequest, cancellationToken)).Rows.Single();

		var metadataSearchRequest = new MetadataSearchRequest
		{
			EntityTypeCode = MasterData.EntityTypeCode.Classifier,
			EntityUid = document.DocumentTypeUid,
			// todo: check flags
			// IsSystem = false,
			IsActive = true,
			SkipPaging = true
		};

		var metadata = (await _fieldMetadataRepository.Search(metadataSearchRequest, cancellationToken)).Rows;

		var manageFieldDataRequest = new ManageFieldDataRequest
		{
			EntityTypeCode = EntityTypeCode.Document,
			// ReSharper disable once PossibleInvalidOperationException
			EntityUid = document.Uid.Value,
			Metadata = metadata,
			Item = request
		};

		var result = await _fieldDataRepository.Validate(manageFieldDataRequest, cancellationToken);

		if (result.Success == false) return result;

		using (var scope = _unitOfWorkFactory.Create())
		{
			result = await _fieldDataRepository.Update(manageFieldDataRequest, cancellationToken);

			if (result.Success) scope.Commit();

			return result;
		}
	}
}