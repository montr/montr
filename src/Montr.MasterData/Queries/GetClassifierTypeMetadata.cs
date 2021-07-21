using MediatR;
using Montr.Metadata.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierTypeMetadata : MetadataRequest, IRequest<DataView>
	{
		public string TypeCode { get; set; }
	}
}
