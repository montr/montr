using System;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Tendr.Queries
{
	public class GetEventMetadata : MetadataRequest, IRequest<DataView>
	{
		public Guid EventUid { get; set; }
	}
}
