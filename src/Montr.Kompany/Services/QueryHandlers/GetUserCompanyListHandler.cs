using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Kompany.Entities;
using Montr.Kompany.Models;
using Montr.Kompany.Queries;
using Montr.MasterData.Impl.Entities;

namespace Montr.Kompany.Services.QueryHandlers
{
	public class GetUserCompanyListHandler : IRequestHandler<GetUserCompanyList, ICollection<Company>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetUserCompanyListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ICollection<Company>> Handle(GetUserCompanyList request, CancellationToken cancellationToken)
		{
			var userUid = request.UserUid;

			using (var db = _dbContextFactory.Create())
			{
				var query = from company in db.GetTable<DbCompany>()
					join classifier in db.GetTable<DbClassifier>() on company.Uid equals classifier.Uid
					join companyUser in db.GetTable<DbCompanyUser>() on company.Uid equals companyUser.CompanyUid
					orderby classifier.Name
					where companyUser.UserUid == userUid
					select classifier;

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
