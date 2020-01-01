using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Queries
{
	public class GetMetadataList : MetadataSearchRequest, IRequest<SearchResult<FieldMetadata>>
	{
	}
}
