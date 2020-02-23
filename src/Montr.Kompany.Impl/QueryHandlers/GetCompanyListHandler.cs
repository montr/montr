using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Data.Linq2Db;
using Montr.Kompany.Impl.Entities;
using Montr.Kompany.Models;
using Montr.Kompany.Queries;

namespace Montr.Kompany.Impl.QueryHandlers
{
	public class GetCompanyListHandler : IRequestHandler<GetCompanyList, IList<Company>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetCompanyListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public Task<IList<Company>> Handle(GetCompanyList request, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				var query = from c in db.GetTable<DbCompany>()
					join cu in db.GetTable<DbCompanyUser>()
						on c.Uid equals cu.CompanyUid
					orderby c.Name
					where cu.UserUid == request.UserUid
					select c;

				var result = query
					.Select(x => new Company
					{
						Uid = x.Uid,
						ConfigCode = x.ConfigCode,
						StatusCode = x.StatusCode,
						Name = x.Name
					}).ToList();

				return Task.FromResult((IList<Company>)result);
			}
		}
	}
}
