﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Entities;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services.Implementations
{
	public class DbClassifierTypeRepository : IRepository<ClassifierType>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbClassifierTypeRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<ClassifierType>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (ClassifierTypeSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var all = db.GetTable<DbClassifierType>().AsQueryable();

				if (request.Code != null)
				{
					all = all.Where(x => x.Code == request.Code);
				}

				if (request.Uid != null)
				{
					all = all.Where(x => x.Uid == request.Uid);
				}

				var paged = all.Apply(request, x => x.Name);

				var data = await paged
					.Select(x => new ClassifierType
					{
						Uid = x.Uid,
						Code = x.Code,
						Name = x.Name,
						Description = x.Description,
						HierarchyType = Enum.Parse<HierarchyType>(x.HierarchyType),
						IsSystem = true,
						Url = $"/classifiers/{x.Code}/"
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<ClassifierType>
				{
					TotalCount = all.GetTotalCount(request),
					Rows = data
				};
			}
		}
	}
}
