using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;

namespace Montr.MasterData.Impl.Services
{
	public class DbNumeratorEntityRepository : IRepository<NumeratorEntity>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly INamedServiceFactory<IEntityNameResolver> _entityNameResolverFactory;

		public DbNumeratorEntityRepository(IDbContextFactory dbContextFactory, INamedServiceFactory<IEntityNameResolver> entityNameResolverFactory)
		{
			_dbContextFactory = dbContextFactory;
			_entityNameResolverFactory = entityNameResolverFactory;
		}

		public async Task<SearchResult<NumeratorEntity>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (NumeratorEntitySearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var dbNumerators = db.GetTable<DbNumerator>().AsQueryable();

				if (request.EntityTypeCode != null)
				{
					dbNumerators = dbNumerators.Where(x => x.EntityTypeCode == request.EntityTypeCode);
				}

				if (request.NumeratorUid != null)
				{
					dbNumerators = dbNumerators.Where(x => x.Uid == request.NumeratorUid);
				}

				var dbNumeratorEntities = db.GetTable<DbNumeratorEntity>().AsQueryable();

				if (request.EntityUid != null)
				{
					dbNumeratorEntities = dbNumeratorEntities.Where(x => x.EntityUid == request.EntityUid);
				}

				var data = await (
						from ne in dbNumeratorEntities
						join n in dbNumerators on ne.NumeratorUid equals n.Uid into ln
						from lne in ln.DefaultIfEmpty()
						where request.NumeratorUid == null || lne.Uid != null // if passed NumeratorUid then do right join
						select new NumeratorEntity
						{
							IsAutoNumbering = ne.IsAutoNumbering,
							NumeratorUid = lne.Uid,
							EntityTypeCode = lne.EntityTypeCode,
							EntityUid = ne.EntityUid
						})
					.ToListAsync(cancellationToken);

				foreach (var entity in data.Where(x => x.EntityTypeCode != null))
				{
					var entityNameResolver = _entityNameResolverFactory.GetRequiredService(entity.EntityTypeCode);

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
