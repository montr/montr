using System;
using System.Threading;
using System.Threading.Tasks;
using Kompany.Implementation.Entities;
using Kompany.Models;
using Kompany.Requests;
using LinqToDB;
using MediatR;
using Montr.Data.Linq2Db;

namespace Kompany.Implementation.RequestHandlers
{
	public class CreateCompanyHandler : IRequestHandler<CreateCompany, Guid>
	{
		public async Task<Guid> Handle(CreateCompany request, CancellationToken cancellationToken)
		{
			var item = request.Company;

			// компания + дата изменения
			// пользователь компании
			// заявка на регистрацию + дата изменения
			// авто-допуск заявки
			// история изменений
			// оповещение оператора
			// оповещение компании

			using (var db = new DbContext())
			{
				// var id = db.Execute<long>("select nextval('company_id_seq')");

				var uid = Guid.NewGuid();

				var result = await db.GetTable<DbCompany>()
					.Value(x => x.Uid, uid)
					.Value(x => x.ConfigCode, item.ConfigCode)
					.Value(x => x.StatusCode, CompanyStatusCode.Draft)
					.Value(x => x.Name, item.Name)
					.InsertAsync(cancellationToken);

				return uid;
			}
		}
	}
}
