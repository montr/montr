using System;
using System.Collections.Generic;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Automate.Queries
{
	public class GetFieldAutomationConditionFieldList : IRequest<IList<FieldMetadata>>
	{
		public string MetadataEntityTypeCode { get; set; }

		public Guid MetadataEntityUid { get; set; }
	}
}
