using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierTypeListHandler : IRequestHandler<GetClassifierTypeList, DataResult<ClassifierType>>
	{
		private readonly IEntityRepository<ClassifierType> _repository;

		public GetClassifierTypeListHandler(IEntityRepository<ClassifierType> repository)
		{
			_repository = repository;
		}

		public async Task<DataResult<ClassifierType>> Handle(GetClassifierTypeList command, CancellationToken cancellationToken)
		{
			return await _repository.Search(command.Request, cancellationToken);
		}
	}
}
