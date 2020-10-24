using MediatR;
using Montr.Metadata.Models;

namespace Montr.Metadata.Queries
{
	public class GetMetadata : MetadataRequest, IRequest<DataView>
	{
	}
}
