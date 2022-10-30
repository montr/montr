using System.Collections.Generic;
using MediatR;
using Montr.Automate.Models;
using Montr.Metadata.Models;

namespace Montr.Automate.Queries
{
	public class GetAutomationConditionMetadata : IRequest<IList<FieldMetadata>>
	{
		public string EntityTypeCode { get; set; }

		public AutomationCondition Condition { get; set; }
	}
}
