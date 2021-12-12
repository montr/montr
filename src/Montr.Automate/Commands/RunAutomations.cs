using System;
using System.Diagnostics;
using MediatR;
using Montr.Core.Models;

namespace Montr.Automate.Commands
{
	// should not be derived from AutomationContext to prevent serialization of property Values in Hangfire job
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class RunAutomations : /*AutomationContext,*/ IRequest<ApiResult>
	{
		private string DebuggerDisplay => $"{GetType().FullName}, " +
		                                  $"MetadataEntityTypeCode: {MetadataEntityTypeCode}, " +
		                                  $"MetadataEntityUid: {MetadataEntityUid}, " +
		                                  $"EntityTypeCode: {EntityTypeCode}, " +
		                                  $"EntityUid: {EntityUid}";

		public string MetadataEntityTypeCode { get; set; }

		public Guid MetadataEntityUid { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		/*public override string ToString()
		{
			return DebuggerDisplay;
		}*/
	}
}
