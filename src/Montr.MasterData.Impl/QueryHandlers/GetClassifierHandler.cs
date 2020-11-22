using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierHandler : IRequestHandler<GetClassifier, Classifier>
	{
		private readonly IClassifierRepository _repository;

		public GetClassifierHandler(IClassifierRepository repository)
		{
			_repository = repository;
		}

		public async Task<Classifier> Handle(GetClassifier request, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new ClassifierSearchRequest
			{
				CompanyUid = request.CompanyUid,
				TypeCode = request.TypeCode,
				Uid = request.Uid,
				IncludeFields = true,
				SkipPaging = true
			}, cancellationToken);

			return result.Rows.Single();
		}
	}
}
