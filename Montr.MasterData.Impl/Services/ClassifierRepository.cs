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
	public class ClassifierRepository : IEntityRepository<Classifier>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public ClassifierRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<DataResult<Classifier>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (ClassifierSearchRequest)searchRequest;

			using (var db = _dbContextFactory.Create())
			{
				var all = db.GetTable<DbClassifier>()
					.Where(x => x.ConfigCode == request.ConfigCode &&
								x.CompanyUid == request.CompanyUid);

				var data = await all
					.Apply(request, x => x.Code)
					.Select(x => new Classifier
					{
						Uid = x.Uid,
						ConfigCode = x.ConfigCode,
						StatusCode = x.StatusCode,
						Code = x.Code,
						Name = x.Name,
						Url = $"/classifiers/{x.ConfigCode}/edit/{x.Uid}"
					})
					.ToListAsync(cancellationToken);

				return new DataResult<Classifier>
				{
					TotalCount = all.Count(),
					Rows = data
				};
			}
		}
	}
}
