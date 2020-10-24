using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Metadata.Models;
using Montr.Metadata.Queries;
using Montr.Metadata.Services;

namespace Montr.Metadata.Impl.QueryHandlers
{
	public class GetMetadataHandler : IRequestHandler<GetMetadata, DataView>
	{
		private readonly IMetadataProvider _metadataProvider;

		public GetMetadataHandler(IMetadataProvider metadataProvider)
		{
			_metadataProvider = metadataProvider;
		}

		public async Task<DataView> Handle(GetMetadata request, CancellationToken cancellationToken)
		{
			return await _metadataProvider.GetView(request.ViewId);
		}
	}
}
