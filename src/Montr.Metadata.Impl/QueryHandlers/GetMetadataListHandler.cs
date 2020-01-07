using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Queries;

namespace Montr.Metadata.Impl.QueryHandlers
{
	public class GetMetadataListHandler : IRequestHandler<GetMetadataList, SearchResult<FieldMetadata>>
	{
		private readonly IRepository<FieldMetadata> _repository;

		public GetMetadataListHandler(IRepository<FieldMetadata> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<FieldMetadata>> Handle(GetMetadataList request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			return await _repository.Search(request, cancellationToken);
		}
	}
}
