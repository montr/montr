using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Services.QueryHandlers
{
	public class GetNumeratorEntityHandler : IRequestHandler<GetNumeratorEntity, NumeratorEntity>
	{
		private readonly IRepository<NumeratorEntity> _repository;

		public GetNumeratorEntityHandler(IRepository<NumeratorEntity> repository)
		{
			_repository = repository;
		}

		public async Task<NumeratorEntity> Handle(GetNumeratorEntity request, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new NumeratorEntitySearchRequest
			{
				// EntityTypeCode = request.EntityTypeCode,
				EntityUid = request.EntityUid
			}, cancellationToken);

			return result.Rows.SingleOrDefault();
		}
	}
}
