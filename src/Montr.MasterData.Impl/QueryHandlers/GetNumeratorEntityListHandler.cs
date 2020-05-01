using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetNumeratorEntityListHandler : IRequestHandler<GetNumeratorEntityList, SearchResult<NumeratorEntity>>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly INamedServiceFactory<IEntityNameResolver> _entityNameResolverFactory;

		public GetNumeratorEntityListHandler(IDbContextFactory dbContextFactory, INamedServiceFactory<IEntityNameResolver> entityNameResolverFactory)
		{
			_dbContextFactory = dbContextFactory;
			_entityNameResolverFactory = entityNameResolverFactory;
		}

		public async Task<SearchResult<NumeratorEntity>> Handle(GetNumeratorEntityList searchRequest, CancellationToken cancellationToken)
		{
			var request = (NumeratorEntitySearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var data = await (
						from n in db.GetTable<DbNumerator>().Where(x => x.Uid == request.NumeratorUid)
						join ne in db.GetTable<DbNumeratorEntity>() on n.Uid equals ne.NumeratorUid
						select new NumeratorEntity
						{
							NumeratorUid = n.Uid,
							EntityTypeCode = n.EntityTypeCode,
							EntityUid = ne.EntityUid
						})
					.ToListAsync(cancellationToken);

				foreach (var entity in data)
				{
					var entityNameResolver = _entityNameResolverFactory.Resolve(entity.EntityTypeCode);

					var entityName = await entityNameResolver.Resolve(entity.EntityTypeCode, entity.EntityUid, cancellationToken);

					if (entityName != null)
					{
						entity.EntityName = entityName;
					}
					else
					{
						entity.EntityName = entity.EntityTypeCode + "@" + entity.EntityUid;
					}
				}

				return new SearchResult<NumeratorEntity>
				{
					Rows = data
				};
			}
		}
	}
}
