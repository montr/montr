using MediatR;
using Montr.Core.Models;
using Montr.Metadata.Models;

namespace Montr.Metadata.Queries
{
	public class GetMetadataList : MetadataSearchRequest, IRequest<SearchResult<FieldMetadata>>
	{
	}
}
