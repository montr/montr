using System.Collections.Generic;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Automate.Queries
{
	public class GetAutomationMetadata : MetadataSearchRequest, IRequest<IList<FieldMetadata>>
	{
		public string ActionTypeCode { get; set; }
	}
}
