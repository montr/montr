using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kompany.Implementation.Entities;
using Kompany.Models;
using Kompany.Queries;
using MediatR;
using Montr.Data.Linq2Db;

namespace Kompany.Implementation.QueryHandlers
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
			if (request.UserUid == Guid.Empty)
				throw new InvalidOperationException("UserUid can't be empty guid.");

			using (var db = _dbContextFactory.Create())
			{
				var query = from c in db.GetTable<DbCompany>()
					join cu in db.GetTable<DbCompanyUser>()
						on c.Uid equals cu.CompanyUid
					where cu.UserUid == request.UserUid
					select c;

				var result = query
					.Select(x => new Company
					{
						Uid = x.Uid,
						ConfigCode = x.ConfigCode,
						StatusCode = x.StatusCode,
						Name = x.Name,
					}).ToList();

				return Task.FromResult((IList<Company>)result);
			}
		}
	}
}
