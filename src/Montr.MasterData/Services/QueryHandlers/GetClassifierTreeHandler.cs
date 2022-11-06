using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Services.QueryHandlers
{
	public class GetClassifierTreeHandler : IRequestHandler<GetClassifierTree, ClassifierTree>
	{
		private readonly IRepository<ClassifierTree> _repository;

		public GetClassifierTreeHandler(IRepository<ClassifierTree> repository)
		{
			_repository = repository;
		}

		public async Task<ClassifierTree> Handle(GetClassifierTree command, CancellationToken cancellationToken)
		{
			var request = new ClassifierTreeSearchRequest
			{
				TypeCode = command.TypeCode,
				Uid = command.Uid
			};

			var types = await _repository.Search(request, cancellationToken);

			return types.Rows.Single();
		}
	}
}
