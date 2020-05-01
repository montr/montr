using System;
using System.Collections.Generic;
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
	public class DbNumeratorRepository : IRepository<Numerator>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbNumeratorRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<Numerator>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (NumeratorSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var query = db.GetTable<DbNumerator>().AsQueryable();

				if (request.EntityTypeCode != null && request.EntityTypeUid != null)
				{
					query = from n in query.Where(x => x.EntityTypeCode == request.EntityTypeCode)
						join ne in db.GetTable<DbNumeratorEntity>().Where(x => x.EntityUid == request.EntityTypeUid)
							on n.Uid equals ne.NumeratorUid
						select n;
				}

				if (request.Uid != null)
				{
					query = query.Where(x => x.Uid == request.Uid);
				}

				var paged = query.Apply(request, x => x.Name);

				var data = await Materialize(paged, cancellationToken); 

				return new SearchResult<Numerator>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}

		private static async Task<List<Numerator>> Materialize(IQueryable<DbNumerator> query, CancellationToken cancellationToken)
		{
			return await query
				.Select(x => new Numerator
				{
					Uid = x.Uid,
					EntityTypeCode = x.EntityTypeCode,
					Name = x.Name,
					Periodicity = Enum.Parse<NumeratorPeriodicity>(x.Periodicity),
					Pattern = x.Pattern,
					KeyTags = x.KeyTags != null
						? x.KeyTags.Split(DbNumerator.KeyTagsSeparator, StringSplitOptions.RemoveEmptyEntries)
						: null,
					IsActive = x.IsActive,
					IsSystem = x.IsSystem,
					Url = $"/numerators/edit/{x.Uid}/"
				})
				.ToListAsync(cancellationToken);
		}
	}
}
