using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.Metadata.Models;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierListHandler : IRequestHandler<GetClassifierList, DataResult<Classifier>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetClassifierListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<DataResult<Classifier>> Handle(GetClassifierList command, CancellationToken cancellationToken)
		{
			var request = command.Request;

			using (var db = _dbContextFactory.Create())
			{
				var all = db.GetTable<DbClassifier>()
					.Where(x => x.ConfigCode == request.ConfigCode &&
								x.CompanyUid == request.CompanyUid);

				var data = await all
					.Apply(request, x => x.Name)
					.Select(x => new Classifier
					{
						Uid = x.Uid,
						ConfigCode = x.ConfigCode,
						StatusCode = x.StatusCode,
						Code = x.Code,
						Name = x.Name
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
