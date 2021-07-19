using System;
using System.Security.Claims;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Docs.Queries
{
	public class GetDocumentMetadata : MetadataRequest, IRequest<DataView>
	{
		public ClaimsPrincipal Principal { get; set; }

		public Guid UserUid { get; set; }

		public Guid DocumentUid { get; set; }
	}
}
