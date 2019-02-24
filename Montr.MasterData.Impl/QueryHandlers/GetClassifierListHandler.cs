using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierListHandler : IRequestHandler<GetClassifierList, DataResult<Classifier>>
	{
		private readonly IEntityRepository<Classifier> _repository;

		public GetClassifierListHandler(IEntityRepository<Classifier> repository)
		{
			_repository = repository;
		}

		public async Task<DataResult<Classifier>> Handle(GetClassifierList command, CancellationToken cancellationToken)
		{
			return await _repository.Search(command.Request, cancellationToken);
		}
	}
}
