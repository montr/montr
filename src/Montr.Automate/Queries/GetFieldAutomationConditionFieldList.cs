using System.Collections.Generic;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Automate.Queries
{
	public class GetFieldAutomationConditionFieldList : IRequest<IList<FieldMetadata>>
	{
		public string EntityTypeCode { get; set; }

		// public Guid EntityUid { get; set; }
	}
}
