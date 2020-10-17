using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;

namespace Montr.Core.Impl.QueryHandlers
{
	public class GetEntityStatusHandler : IRequestHandler<GetEntityStatus, EntityStatus>
	{
		private readonly IRepository<EntityStatus> _repository;

		public GetEntityStatusHandler(IRepository<EntityStatus> repository)
		{
			_repository = repository;
		}

		public async Task<EntityStatus> Handle(GetEntityStatus request, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new EntityStatusSearchRequest
			{
				EntityTypeCode = request.EntityTypeCode,
				EntityUid = request.EntityUid,
				Uid = request.Uid
			}, cancellationToken);

			return result.Rows.Single();
		}
	}
}
