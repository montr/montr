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

namespace Montr.Kompany.Impl.CommandHandlers
{
	public class CreateCompanyHandler : IRequestHandler<CreateCompany, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IDocumentRepository _documentRepository;
		private readonly IAuditLogService _auditLogService;

		public CreateCompanyHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IDateTimeProvider dateTimeProvider, IDocumentRepository documentRepository, IAuditLogService auditLogService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
			_documentRepository = documentRepository;
			_auditLogService = auditLogService;
		}

		public async Task<ApiResult> Handle(CreateCompany request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("UserUid can't be empty guid.");

			var company = request.Company ?? throw new ArgumentNullException(nameof(request.Company));

			var now = _dateTimeProvider.GetUtcNow();

			var companyUid = Guid.NewGuid();

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
						.Value(x => x.UserUid, request.UserUid)
						.InsertAsync(cancellationToken);
				}

				// todo: user roles 

				// заявка на регистрацию + todo: дата изменения
				await _documentRepository.Create(new Document
				{
					CompanyUid = companyUid,
					ConfigCode = CompanyRequestConfigCode.RegistrationRequest
				});

				// todo: история изменений всего
				await _auditLogService.Save(new AuditEvent
				{
					EntityTypeCode = "company",
					EntityUid = companyUid,
					CompanyUid = companyUid,
					UserUid = request.UserUid,
					CreatedAtUtc = now,
					MessageCode = "Company.Created"
				});

				// todo: (через события в фоне) авто-допуск заявки, оповещения для оператора и компании

				scope.Commit();

				return new ApiResult { Uid = companyUid };
			}
		}
	}
}
