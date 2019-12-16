using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;

namespace Montr.Core.Impl.QueryHandlers
{
	public class GetMetadataListHandler : IRequestHandler<GetMetadataList, SearchResult<DataField>>
	{
		private readonly IRepository<DataField> _repository;

		public GetMetadataListHandler(IRepository<DataField> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<DataField>> Handle(GetMetadataList request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			return await _repository.Search(request, cancellationToken);
		}
	}
}
