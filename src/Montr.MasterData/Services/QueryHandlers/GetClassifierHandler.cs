using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Services.QueryHandlers
{
	public class GetClassifierHandler : IRequestHandler<GetClassifier, Classifier>
	{
		private readonly INamedServiceFactory<IClassifierRepository> _repositoryFactory;

		public GetClassifierHandler(INamedServiceFactory<IClassifierRepository> repositoryFactory)
		{
			_repositoryFactory = repositoryFactory;
		}

		public async Task<Classifier> Handle(GetClassifier request, CancellationToken cancellationToken)
		{
			var repository = _repositoryFactory.GetNamedOrDefaultService(request.TypeCode);

			var result = await repository.Search(new ClassifierSearchRequest
			{
				TypeCode = request.TypeCode,
				Uid = request.Uid,
				IncludeFields = true,
				SkipPaging = true
			}, cancellationToken);

			return result.Rows.Single();
		}
	}
}
