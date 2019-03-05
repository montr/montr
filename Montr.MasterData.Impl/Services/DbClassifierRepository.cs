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
	public class DbClassifierRepository : IRepository<Classifier>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbClassifierRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<Classifier>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (ClassifierSearchRequest)searchRequest;

			using (var db = _dbContextFactory.Create())
			{
				var all = from c in db.GetTable<DbClassifier>()
					join ct in db.GetTable<DbClassifierType>() on c.TypeUid equals ct.Uid
					where c.CompanyUid == request.CompanyUid && ct.Code == request.TypeCode
					select c;

				var data = await all
					.Apply(request, x => x.Code)
					.Select(x => new Classifier
					{
						Uid = x.Uid,
						StatusCode = x.StatusCode,
						Code = x.Code,
						Name = x.Name,
						// Url = $"/classifiers/{x.ConfigCode}/edit/{x.Uid}"
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<Classifier>
				{
					TotalCount = all.Count(),
					Rows = data
				};
			}
		}
	}
}
