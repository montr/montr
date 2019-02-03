using System;
using System.Threading;
using System.Threading.Tasks;
using Dokumento.Models;
using Dokumento.Services;
using Kompany.Commands;
using Kompany.Implementation.Entities;
using Kompany.Models;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Kompany.Implementation.CommandHandlers
{
	public class CreateCompanyHandler : IRequestHandler<CreateCompany, Guid>
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

		public async Task<Guid> Handle(CreateCompany request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty)
				throw new InvalidOperationException("UserUid can't be empty guid.");

			var now = _dateTimeProvider.GetUtcNow();

			var company = request.Company;

			company.Uid = Guid.NewGuid();
			company.StatusCode = CompanyStatusCode.Draft;

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					// todo: валидация и ограничения

					// компания + todo: дата изменения
					await db.GetTable<DbCompany>()
						.Value(x => x.Uid, company.Uid)
						.Value(x => x.ConfigCode, company.ConfigCode)
						.Value(x => x.StatusCode, company.StatusCode)
						.Value(x => x.Name, company.Name)
						.InsertAsync(cancellationToken);

					// пользователь в компании
					await db.GetTable<DbCompanyUser>()
						.Value(x => x.CompanyUid, company.Uid)
						.Value(x => x.UserUid, request.UserUid)
						.InsertAsync(cancellationToken);
				}

				// todo: роли пользователя

				// заявка на регистрацию + todo: дата изменения
				await _documentRepository.Create(new Document
				{
					CompanyUid = company.Uid,
					ConfigCode = CompanyRequestConfigCode.RegistrationRequest
				});

				// todo: история изменений всего
				await _auditLogService.Save(new AuditEvent
				{
					EntityTypeCode = "company",
					EntityUid = company.Uid,
					CompanyUid = company.Uid,
					UserUid = request.UserUid,
					CreatedAtUtc = now,
					MessageCode = "Company.Created"
				});

				// todo: (через события в фоне) авто-допуск заявки, оповещения для оператора и компании

				scope.Commit();

				return company.Uid;
			}
		}
	}
}
