using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Data.Linq2Db;
using Montr.Kompany.Impl.Entities;
using Montr.Kompany.Models;
using Montr.Kompany.Queries;
using Montr.MasterData.Impl.Entities;

namespace Montr.Kompany.Impl.QueryHandlers
{
	public class GetCompanyListHandler : IRequestHandler<GetUserCompanyList, ICollection<Company>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetCompanyListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ICollection<Company>> Handle(GetUserCompanyList request, CancellationToken cancellationToken)
		{
			var userUid = request.UserUid;

			using (var db = _dbContextFactory.Create())
			{
				var query = from company in db.GetTable<DbCompany>()
					join c in db.GetTable<DbClassifier>() on company.Uid equals c.Uid
					join cu in db.GetTable<DbCompanyUser>()
						on company.Uid equals cu.CompanyUid
					orderby c.Name
					where cu.UserUid == userUid
					select c;

				var result = await query.Select(x => new Company
				{
					Uid = x.Uid,
					Name = x.Name
				}).ToListAsync(cancellationToken);

				return result;
			}
		}
	}
}
