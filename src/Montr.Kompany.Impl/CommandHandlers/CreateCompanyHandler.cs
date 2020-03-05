using System;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Docs.Models;
using Montr.Docs.Services;
using Montr.Kompany.Commands;
using Montr.Kompany.Impl.Entities;
using Montr.Kompany.Models;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Kompany.Impl.CommandHandlers
{
	public class CreateCompanyHandler : IRequestHandler<CreateCompany, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IRepository<FieldMetadata> _fieldMetadataRepository;
		private readonly IFieldDataRepository _fieldDataRepository;
		private readonly IDocumentRepository _documentRepository;
		private readonly IAuditLogService _auditLogService;

		public CreateCompanyHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IDateTimeProvider dateTimeProvider, IRepository<FieldMetadata> fieldMetadataRepository, IFieldDataRepository fieldDataRepository,
			IDocumentRepository documentRepository, IAuditLogService auditLogService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
			_fieldMetadataRepository = fieldMetadataRepository;
			_fieldDataRepository = fieldDataRepository;
			_documentRepository = documentRepository;
			_auditLogService = auditLogService;
		}

		public async Task<ApiResult> Handle(CreateCompany request, CancellationToken cancellationToken)
		{
			var userUid = request.UserUid ?? throw new ArgumentNullException(nameof(request.UserUid));
			var company = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var now = _dateTimeProvider.GetUtcNow();

			var companyUid = Guid.NewGuid();
			var documentUid = Guid.NewGuid();

			// todo: validate fields
			var metadata = await _fieldMetadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = Process.EntityTypeCode,
				EntityUid = Process.Registration,
				// todo: check flags
				// IsSystem = false,
				IsActive = true,
				SkipPaging = true
			}, cancellationToken);

			var manageFieldDataRequest = new ManageFieldDataRequest
			{
				EntityTypeCode = Document.EntityTypeCode,
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

					// компания + todo: дата изменения
					await db.GetTable<DbCompany>()
						.Value(x => x.Uid, companyUid)
						.Value(x => x.ConfigCode, company.ConfigCode ?? CompanyConfigCode.Company)
						.Value(x => x.StatusCode, CompanyStatusCode.Draft)
						.Value(x => x.Name, company.Name)
						.InsertAsync(cancellationToken);

					// пользователь в компании
					await db.GetTable<DbCompanyUser>()
						.Value(x => x.CompanyUid, companyUid)
						.Value(x => x.UserUid, userUid)
						.InsertAsync(cancellationToken);
				}

				// insert fields
				// todo: exclude db fields and sections
				await _fieldDataRepository.Insert(manageFieldDataRequest, cancellationToken);

				// todo: user roles 

				// company registration request + todo: дата изменения
				await _documentRepository.Create(new Document
				{
					Uid = documentUid,
					CompanyUid = companyUid,
					ConfigCode = CompanyRequestConfigCode.RegistrationRequest,
					Direction = DocumentDirection.Outgoing,
					DocumentNumber = companyUid.ToString(), // todo: generate number
					DocumentDate = now
				}, cancellationToken);

				// todo: audit log for company and for document
				await _auditLogService.Save(new AuditEvent
				{
					EntityTypeCode = Company.EntityTypeCode,
					EntityUid = companyUid,
					CompanyUid = companyUid,
					UserUid = userUid,
					CreatedAtUtc = now,
					MessageCode = ExpressionHelper.GetFullName<CreateCompany.Resources>(x => x.CompanyCreated)
				});

				// todo: (через события в фоне) авто-допуск заявки, оповещения для оператора и компании

				scope.Commit();

				return new ApiResult { Uid = companyUid };
			}
		}
	}
}
