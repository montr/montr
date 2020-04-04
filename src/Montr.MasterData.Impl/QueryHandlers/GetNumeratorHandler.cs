using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetNumeratorHandler : IRequestHandler<GetNumerator, Numerator>
	{
		private readonly IRepository<Numerator> _repository;

		public GetNumeratorHandler(IRepository<Numerator> repository)
		{
			_repository = repository;
		}

		public async Task<Numerator> Handle(GetNumerator request, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new NumeratorSearchRequest
			{
				Uid = request.Uid
			}, cancellationToken);

			return result.Rows.Single();
		}
	}
}
