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

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierHandler : IRequestHandler<GetClassifier, Classifier>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<Classifier> _repository;

		public GetClassifierHandler(IDbContextFactory dbContextFactory, IRepository<Classifier> repository)
		{
			_dbContextFactory = dbContextFactory;
			_repository = repository;
		}

		public async Task<Classifier> Handle(GetClassifier request, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new ClassifierSearchRequest
				{
					CompanyUid = request.CompanyUid,
					Uid = request.EntityUid
				},
				cancellationToken);

			return result.Rows.SingleOrDefault();

			/*using (var db = _dbContextFactory.Create())
			{
				// todo: use repository
				var result = await db.GetTable<DbClassifier>()
					.Where(x => x.Uid == request.EntityUid)
					.Select(x => new Classifier
					{
						Uid = x.Uid,
						TypeCode = x.ConfigCode,
						StatusCode = x.StatusCode,
						Code = x.Code,
						Name = x.Name,
						Url = $"/classifiers/{x.ConfigCode}/edit/{x.Uid}"
					})
					.SingleAsync(cancellationToken);

				return result;
			}*/
		}
	}
}
