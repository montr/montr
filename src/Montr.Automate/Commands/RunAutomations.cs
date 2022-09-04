using System;
using System.Diagnostics;
using MediatR;
using Montr.Core.Models;

namespace Montr.Automate.Commands
{
	// Should not be derived from AutomationContext to prevent serialization of property Values in Hangfire job
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class RunAutomations : IRequest<ApiResult>
	{
		private string DebuggerDisplay => $"{GetType().FullName}, " +
		                                  $"EntityTypeCode: {EntityTypeCode}, " +
		                                  $"EntityUid: {EntityUid}";

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }
	}
}
