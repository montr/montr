using System;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Automate.Commands;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Docs;
using Montr.Docs.Models;
using Montr.Docs.Services;
using Montr.Kompany.Commands;
using Montr.Kompany.Impl.Entities;
using Montr.Kompany.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;
using Montr.Worker.Services;

namespace Montr.Kompany.Impl.CommandHandlers
{
	public class CreateCompanyHandler : IRequestHandler<CreateCompany, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IRepository<FieldMetadata> _fieldMetadataRepository;
		private readonly IFieldDataRepository _fieldDataRepository;
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;
		private readonly IDocumentService _documentService;
		private readonly IAuditLogService _auditLogService;
		private readonly IBackgroundJobManager _jobManager;

		public CreateCompanyHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IDateTimeProvider dateTimeProvider, IRepository<FieldMetadata> fieldMetadataRepository, IFieldDataRepository fieldDataRepository,
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory, IDocumentService documentService,
			IAuditLogService auditLogService, IBackgroundJobManager jobManager)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
			_fieldMetadataRepository = fieldMetadataRepository;
			_fieldDataRepository = fieldDataRepository;
			_classifierRepositoryFactory = classifierRepositoryFactory;
			_documentService = documentService;
			_auditLogService = auditLogService;
			_jobManager = jobManager;
		}

		public async Task<ApiResult> Handle(CreateCompany request, CancellationToken cancellationToken)
		{
			var userUid = request.UserUid ?? throw new ArgumentNullException(nameof(request.UserUid));
			var company = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var now = _dateTimeProvider.GetUtcNow();

			company.Uid ??= Guid.NewGuid();

			var documentUid = Guid.NewGuid();

			var documentTypeRepository = _classifierRepositoryFactory.GetNamedOrDefaultService(Docs.ClassifierTypeCode.DocumentType);
			var documentType = await documentTypeRepository.Get(Docs.ClassifierTypeCode.DocumentType, DocumentTypes.CompanyRegistrationRequest, cancellationToken);

			var companyRepository = _classifierRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.Company);

			// todo: validate fields
			var metadata = await _fieldMetadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = DocumentType.EntityTypeCode,
				EntityUid = documentType.Uid.Value,
				// todo: check flags
				// IsSystem = false,
				IsActive = true,
				SkipPaging = true
			}, cancellationToken);

			var manageFieldDataRequest = new ManageFieldDataRequest
			{
				EntityTypeCode = Document.TypeCode,
				EntityUid = documentUid,
				Metadata = metadata.Rows,
				Item = company
			};

			// todo: move to Validator (?)
			var result = await _fieldDataRepository.Validate(manageFieldDataRequest, cancellationToken);

			if (result.Success == false) return result;

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					// todo: валидация и ограничения

					var companyResult = await companyRepository.Insert(company, cancellationToken);

					companyResult.AssertSuccess(() => $"Failed to register company {company}");

					// user in company
					await db.GetTable<DbCompanyUser>()
						.Value(x => x.CompanyUid, company.Uid)
						.Value(x => x.UserUid, userUid)
						.InsertAsync(cancellationToken);
				}

				// insert fields
				// todo: exclude db fields and sections
				await _fieldDataRepository.Insert(manageFieldDataRequest, cancellationToken);

				// todo: user roles

				// company registration request + todo: creation date
				var document = new Document
				{
					Uid = documentUid,
					DocumentTypeUid = documentType.Uid.Value,
					CompanyUid = company.Uid.Value,
					StatusCode = DocumentStatusCode.Published,
					Direction = DocumentDirection.Outgoing,
					DocumentDate = now,
					// Name = $"Company {company.Name} registration request"
				};

				await _documentService.Create(document, cancellationToken);

				// todo: audit log for company and for document
				await _auditLogService.Save(new AuditEvent
				{
					EntityTypeCode = Company.EntityTypeCode,
					EntityUid = company.Uid.Value,
					CompanyUid = company.Uid.Value,
					UserUid = userUid,
					CreatedAtUtc = now,
					MessageCode = ExpressionHelper.GetFullName<CreateCompany.Resources>(x => x.CompanyCreated)
				}, cancellationToken);

				// todo: auto-approve request, notifications
				_jobManager.Enqueue<ISender>(x => x.Send(new RunAutomations
				{
					EntityTypeCode = DocumentType.EntityTypeCode,
					EntityTypeUid = documentType.Uid.Value,
					EntityUid = documentUid
				}, cancellationToken));

				scope.Commit();

				return new ApiResult { Uid = company.Uid };
			}
		}
	}
}
