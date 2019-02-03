using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kompany.Implementation.Entities;
using Kompany.Models;
using Kompany.Queries;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Metadata.Models;

namespace Kompany.Implementation.QueryHandlers
{
	public class GetContractorListHandler : IRequestHandler<GetContractorList, DataResult<Company>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetContractorListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}
		public async Task<DataResult<Company>> Handle(GetContractorList command, CancellationToken cancellationToken)
		{
			var request = command.Request;

			using (var db = _dbContextFactory.Create())
			{
				var all = db.GetTable<DbCompany>();

				var data = await all
					.Apply(request, "Id", SortOrder.Descending)
					.Select(x => new Company
					{
						Uid = x.Uid,
						ConfigCode = x.ConfigCode,
						StatusCode = x.StatusCode,
						Name = x.Name
					})
					.ToListAsync(cancellationToken);

				return new DataResult<Company>
				{
					TotalCount = all.Count(),
					Rows = data
				};
			}
		}
	}
}
