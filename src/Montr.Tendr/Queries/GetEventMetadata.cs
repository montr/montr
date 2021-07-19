using System;
using System.Security.Claims;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Tendr.Queries
{
	public class GetEventMetadata : MetadataRequest, IRequest<DataView>
	{
		public ClaimsPrincipal Principal { get; set; }

		public Guid UserUid { get; set; }

		public Guid EventUid { get; set; }
	}
}
