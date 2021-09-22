using System;
using System.Diagnostics;
using MediatR;
using Montr.Core.Models;

namespace Montr.Automate.Commands
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class RunAutomations : /*AutomationContext,*/ IRequest<ApiResult>
	{
		private string DebuggerDisplay => $"{GetType().FullName}, " +
										$"EntityTypeCode: {EntityTypeCode}, " +
										$"EntityTypeUid: {EntityTypeUid}, " +
										$"EntityUid: {EntityUid}";

		public string EntityTypeCode { get; set; }

		public Guid EntityTypeUid { get; set; }

		public Guid EntityUid { get; set; }

		public override string ToString()
		{
			return DebuggerDisplay;
		}
	}
}
