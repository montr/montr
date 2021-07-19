using System.Security.Claims;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierMetadata : MetadataRequest, IRequest<DataView>
	{
		public ClaimsPrincipal Principal { get; set; }

		public string TypeCode { get; set; }
	}
}
