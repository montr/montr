using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Queries;

namespace Montr.Idx.Impl.QueryHandlers
{
	public class GetRolePermissionsHandler : IRequestHandler<GetRolePermissions, SearchResult<Permission>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetRolePermissionsHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<Permission>> Handle(GetRolePermissions request, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var roleUid = request?.RoleUid ?? throw new ArgumentNullException(nameof(request));

			using (var db = _dbContextFactory.Create())
			{
				var query =
					from roleClaim in db.GetTable<DbRoleClaim>()
					where roleClaim.RoleId == roleUid && roleClaim.ClaimType == Permission.ClaimType
					select roleClaim;

				var data = await query.Apply(request, x => x.ClaimValue)
					.Select(x => new Permission(x.ClaimValue))
					.ToListAsync(cancellationToken);

				return new SearchResult<Permission>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}
	}
}
