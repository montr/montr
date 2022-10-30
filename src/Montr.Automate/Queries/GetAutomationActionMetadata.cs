using System.Collections.Generic;
using MediatR;
using Montr.Automate.Models;
using Montr.Metadata.Models;

namespace Montr.Automate.Queries
{
	public class GetAutomationActionMetadata : IRequest<IList<FieldMetadata>>
	{
		public string EntityTypeCode { get; set; }
		
		public AutomationAction Action { get; set; }
	}
}
