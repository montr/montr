using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;

namespace Montr.Core.Impl.QueryHandlers
{
	public class GetDataFieldHandler : IRequestHandler<GetDataField, DataField>
	{
		private readonly IRepository<DataField> _repository;

		public GetDataFieldHandler(IRepository<DataField> repository)
		{
			_repository = repository;
		}

		public async Task<DataField> Handle(GetDataField request, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = request.EntityTypeCode,
				Uid = request.Uid
			}, cancellationToken);

			return result.Rows.Single();
		}
	}
}
