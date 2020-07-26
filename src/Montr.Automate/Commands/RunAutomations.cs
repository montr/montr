using System.Diagnostics;
using MediatR;
using Montr.Automate.Models;
using Montr.Core.Models;

namespace Montr.Automate.Commands
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class RunAutomations : AutomationContext, IRequest<ApiResult>
	{
		private string DebuggerDisplay => $"{GetType().FullName}, " +
										$"EntityTypeCode: {EntityTypeCode}, " +
										$"EntityTypeUid: {EntityTypeUid}, " +
										$"EntityUid: {EntityUid}";

		public override string ToString()
		{
			return DebuggerDisplay;
		}
	}
}
