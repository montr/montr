﻿using System.Linq;
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
	public class DbClassifierTreeRepository : IRepository<ClassifierTree>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbClassifierTreeRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<ClassifierTree>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (ClassifierTreeSearchRequest)searchRequest;

			using (var db = _dbContextFactory.Create())
			{
				var query = from trees in db.GetTable<DbClassifierTree>()
							join types in db.GetTable<DbClassifierType>()
								on trees.TypeUid equals types.Uid
							where // types.CompanyUid == request.CompanyUid &&
								  types.Code == request.TypeCode
							select trees;

				if (request.Code != null)
				{
					query = query.Where(x => x.Code == request.Code);
				}

				if (request.Uid != null)
				{
					query = query.Where(x => x.Uid == request.Uid);
				}

				var data = await query
					.Apply(request, x => x.Code)
					.Select(x => new ClassifierTree
					{
						Uid = x.Uid,
						Code = x.Code,
						Name = x.Name
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<ClassifierTree>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}
	}
}
