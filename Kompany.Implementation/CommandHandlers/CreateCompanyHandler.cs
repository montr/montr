using System;
using System.Threading;
using System.Threading.Tasks;
using Kompany.Commands;
using Kompany.Implementation.Entities;
using Kompany.Models;
using LinqToDB;
using MediatR;
using Montr.Data.Linq2Db;
using Montr.Data.Services;

namespace Kompany.Implementation.CommandHandlers
{
	public class CreateCompanyHandler : IRequestHandler<CreateCompany, Guid>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;

		public CreateCompanyHandler(IUnitOfWorkFactory unitOfWorkFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
		}

		public async Task<Guid> Handle(CreateCompany request, CancellationToken cancellationToken)
		{
			var company = request.Company;

			company.Uid = Guid.NewGuid();
			company.StatusCode = CompanyStatusCode.Draft;

			// валидация
			// компания + дата изменения
			// пользователь в компании
			// роли пользователя
			// заявка на регистрацию + дата изменения
			// авто-допуск заявки
			// история изменений
			// оповещение оператора
			// оповещение компании

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = new DbContext())
				{
					// var id = db.Execute<long>("select nextval('company_id_seq')");

					await db.GetTable<DbCompany>()
						.Value(x => x.Uid, company.Uid)
						.Value(x => x.ConfigCode, company.ConfigCode)
						.Value(x => x.StatusCode, company.StatusCode)
						.Value(x => x.Name, company.Name)
						.InsertAsync(cancellationToken);

					await db.GetTable<DbCompanyUser>()
						.Value(x => x.CompanyUid, company.Uid)
						.Value(x => x.UserUid, request.UserUid)
						.InsertAsync(cancellationToken);

					scope.Commit();
				}

				return company.Uid;
			}
		}
	}
}
