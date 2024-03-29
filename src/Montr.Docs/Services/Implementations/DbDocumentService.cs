﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Events;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Commands;
using Montr.Docs.Entities;
using Montr.Docs.Models;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Docs.Services.Implementations
{
	public class DbDocumentService : IDocumentService
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<Document> _documentRepository;
		private readonly INumberGenerator _numberGenerator;
		private readonly IPublisher _mediator;
		private readonly IRepository<FieldMetadata> _fieldMetadataRepository;
		private readonly IFieldDataRepository _fieldDataRepository;

		public DbDocumentService(IDbContextFactory dbContextFactory,
			IRepository<Document> documentRepository, INumberGenerator numberGenerator, IPublisher mediator,
			IRepository<FieldMetadata> fieldMetadataRepository, IFieldDataRepository fieldDataRepository)
		{
			_dbContextFactory = dbContextFactory;
			_documentRepository = documentRepository;
			_numberGenerator = numberGenerator;
			_mediator = mediator;
			_fieldMetadataRepository = fieldMetadataRepository;
			_fieldDataRepository = fieldDataRepository;
		}

		public async Task<ApiResult> Create(Document document, CancellationToken cancellationToken)
		{
			if (document.DocumentDate == DateTime.MinValue) throw new InvalidOperationException("Invalid document date (cannot be min datetime)");

			document.Uid ??= Guid.NewGuid();
			document.StatusCode ??= DocumentStatusCode.Draft;

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbDocument>()
					.Value(x => x.Uid, document.Uid)
					.Value(x => x.DocumentTypeUid, document.DocumentTypeUid)
					.Value(x => x.StatusCode, document.StatusCode)
					.Value(x => x.Direction, document.Direction.ToString().ToLowerInvariant())
					.Value(x => x.DocumentNumber, document.DocumentNumber)
					.Value(x => x.DocumentDate, document.DocumentDate)
					.Value(x => x.CompanyUid, document.CompanyUid)
					.Value(x => x.Name, document.Name)
					.Value(x => x.CreatedAtUtc, document.CreatedAtUtc)
					.Value(x => x.CreatedBy, document.CreatedBy)
					.Value(x => x.Name, document.Name)
					.InsertAsync(cancellationToken);
			}

			// if (document.StatusCode == DocumentStatusCode.Published)
			{
				await GenerateNumber(document, cancellationToken);
			}

			var notification = new EntityStatusChanged<Document>
			{
				Entity = document,
				StatusCode = document.StatusCode
			};

			await _mediator.Publish(notification, cancellationToken);

			return new ApiResult { Uid = document.Uid };
		}

		public async Task<ApiResult> ChangeStatus(Guid documentUid, string statusCode, CancellationToken cancellationToken = default)
		{
			var documentSearchRequest = new DocumentSearchRequest { Uid = documentUid, IncludeFields = true };

			var document = (await _documentRepository.Search(documentSearchRequest, cancellationToken)).Rows.Single();

			// todo: it is not required to load document - load only form of document
			var result = await ValidateForm(document, document, cancellationToken);

			if (result.Success == false) return result;

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbDocument>()
					.Where(x => x.Uid == documentUid)
					.Set(x => x.StatusCode, statusCode)
					.UpdateAsync(cancellationToken);
			}

			await _mediator.Publish(new EntityStatusChanged<Document>
			{
				Entity = document,
				StatusCode = statusCode
			}, cancellationToken);

			return new ApiResult { Success = true };
		}

		// todo: use EntityStatusChanged event (?)
		private async Task<ApiResult> ValidateForm(Document document, IFieldDataContainer data, CancellationToken cancellationToken)
		{
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
				Item = data
			};

			return await _fieldDataRepository.Validate(manageFieldDataRequest, cancellationToken);
		}

		private async Task GenerateNumber(Document document, CancellationToken cancellationToken)
		{
			var documentNumber = await _numberGenerator.GenerateNumber(new GenerateNumberRequest
			{
				EntityTypeCode = DocumentType.EntityTypeCode,
				EntityTypeUid = document.DocumentTypeUid,
				EntityUid = document.Uid
			}, cancellationToken);

			if (documentNumber != null)
			{
				document.DocumentNumber = documentNumber;

				using (var db = _dbContextFactory.Create())
				{
					await db.GetTable<DbDocument>()
						.Where(x => x.Uid == document.Uid)
						.Set(x => x.DocumentNumber, documentNumber)
						.UpdateAsync(cancellationToken);
				}
			}
		}

		public virtual async Task<ApiResult> Delete(DeleteDocument request, CancellationToken cancellationToken)
		{
			ApiResult result;

			using (var db = _dbContextFactory.Create())
			{
				result = await DeleteInternal(db, request, cancellationToken);

				if (result.Success == false) return result;
			}

			return result;
		}

		protected virtual async Task<ApiResult> DeleteInternal(
			DbContext db, DeleteDocument request, CancellationToken cancellationToken = default)
		{
			var documents = db.GetTable<DbDocument>().Where(x => request.Uids.Contains(x.Uid));

			if (request.UserUid != null)
			{
				documents = documents.Where(x => x.CreatedBy == request.UserUid);
			}

			if (request.StatusCode != null)
			{
				documents = documents.Where(x => x.StatusCode == request.StatusCode);
			}

			var affected = await documents.DeleteAsync(cancellationToken);

			return new ApiResult { AffectedRows = affected };
		}
	}
}
