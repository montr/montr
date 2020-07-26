using System;
using System.Diagnostics;
using MediatR;
using Montr.Core.Models;

namespace Montr.Tendr.Commands
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class SendInvitations : IRequest<ApiResult>
	{
		private string DebuggerDisplay => $"{GetType().FullName}, EventUid: {EventUid}";

		public Guid EventUid { get; set; }

		public override string ToString()
		{
			return DebuggerDisplay;
		}
	}
}
