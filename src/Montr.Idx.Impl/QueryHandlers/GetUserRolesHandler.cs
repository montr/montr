using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
using Montr.Idx.Queries;

namespace Montr.Idx.Impl.QueryHandlers
{
	public class GetUserRolesHandler : IRequestHandler<GetUserRoles, SearchResult<Role>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetUserRolesHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<Role>> Handle(GetUserRoles request, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var userUid = request?.UserUid ?? throw new ArgumentNullException(nameof(request));

			using (var db = _dbContextFactory.Create())
			{
				var query =
					from userRole in db.GetTable<DbUserRole>()
					join role in db.GetTable<DbRole>() on userRole.RoleId equals role.Id
					where userRole.UserId == userUid
					select role;

				var data = await Materialize(query.Apply(request, x => x.Name), cancellationToken);

				return new SearchResult<Role>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}

		private static async Task<IList<Role>> Materialize(IQueryable<DbRole> query, CancellationToken cancellationToken)
		{
			return await query.Select(x => new Role
			{
				Uid = x.Id,
				Name = x.Name,
				ConcurrencyStamp = x.ConcurrencyStamp,
				Url = $"/classifiers/role/edit/{x.Id}"
			}).ToListAsync(cancellationToken);
		}
	}
}
