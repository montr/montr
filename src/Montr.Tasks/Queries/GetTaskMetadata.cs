using System;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Tasks.Queries
{
	public class GetTaskMetadata : MetadataRequest, IRequest<DataView>
	{
		public Guid TaskUid { get; set; }
	}
}
