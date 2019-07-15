using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierHandler : IRequestHandler<GetClassifier, Classifier>
	{
		private readonly IRepository<Classifier> _repository;

		public GetClassifierHandler(IRepository<Classifier> repository)
		{
			_repository = repository;
		}

		public async Task<Classifier> Handle(GetClassifier request, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new ClassifierSearchRequest
			{
				CompanyUid = request.CompanyUid,
				TypeCode = request.TypeCode,
				Uid = request.Uid
			}, cancellationToken);

			return result.Rows.Single();
		}
	}
}
