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

		public GetNumeratorEntityListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<NumeratorEntity>> Handle(GetNumeratorEntityList searchRequest, CancellationToken cancellationToken)
		{
			var request = (NumeratorEntitySearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var query = db.GetTable<DbNumeratorEntity>().Where(x => x.NumeratorUid == request.NumeratorUid);

				var paged = query.Apply(request, x => x.EntityUid);

				var data = await paged.Select(x => new NumeratorEntity
					{
						EntityName = x.EntityUid.ToString(),
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<NumeratorEntity>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}
	}
}
