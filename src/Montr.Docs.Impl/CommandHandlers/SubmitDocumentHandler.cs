using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Docs.Commands;
using Montr.Docs.Impl.Entities;
using Montr.Docs.Models;
using Montr.MasterData;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Docs.Impl.CommandHandlers
{
	public class SubmitDocumentHandler : IRequestHandler<SubmitDocument, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<Document> _documentService;
		private readonly IRepository<FieldMetadata> _fieldMetadataRepository;
		private readonly IFieldDataRepository _fieldDataRepository;

		public SubmitDocumentHandler(
			IUnitOfWorkFactory unitOfWorkFactory,
			IDbContextFactory dbContextFactory,
			IRepository<Document> documentService,
			IRepository<FieldMetadata> fieldMetadataRepository,
			IFieldDataRepository fieldDataRepository)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_documentService = documentService;
			_fieldMetadataRepository = fieldMetadataRepository;
			_fieldDataRepository = fieldDataRepository;
		}

		public async Task<ApiResult> Handle(SubmitDocument request, CancellationToken cancellationToken)
		{
			var documentSearchRequest = new DocumentSearchRequest
			{
				Uid = request.DocumentUid ?? throw new ArgumentNullException(nameof(request.DocumentUid)),
				IncludeFields = true
			};

			var document = (await _documentService.Search(documentSearchRequest, cancellationToken)).Rows.Single();

			var result = await ValidateForm(document, document, cancellationToken);

			if (result.Success == false) return result;

			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					affected = await db.GetTable<DbDocument>()
						.Where(x => x.Uid == document.Uid)
						.Set(x => x.StatusCode, DocumentStatusCode.Published)
						.UpdateAsync(cancellationToken);
				}

				var success = affected == 1;

				if (success) scope.Commit();

				return new ApiResult { Success = success, AffectedRows = affected };
			}
		}

		// todo: use EntityStatusChanged event (?)
		private async Task<ApiResult> ValidateForm(Document document, IFieldDataContainer data, CancellationToken cancellationToken)
		{
			var metadataSearchRequest = new MetadataSearchRequest
			{
				EntityTypeCode = EntityTypeCode.Classifier,
				EntityUid = document.DocumentTypeUid,
				// todo: check flags
				// IsSystem = false,
				IsActive = true,
				SkipPaging = true
			};

			var metadata = (await _fieldMetadataRepository.Search(metadataSearchRequest, cancellationToken)).Rows;

			var manageFieldDataRequest = new ManageFieldDataRequest
			{
				EntityTypeCode = Document.TypeCode,
				// ReSharper disable once PossibleInvalidOperationException
				EntityUid = document.Uid.Value,
				Metadata = metadata,
				Item = data
			};

			return await _fieldDataRepository.Validate(manageFieldDataRequest, cancellationToken);
		}
	}
}
