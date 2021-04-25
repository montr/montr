using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Montr.Core.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class PermissionRequirement : IAuthorizationRequirement
	{
		private string DebuggerDisplay => $"{Permission}";

		public string Permission { get; }

		public PermissionRequirement(string permission)
		{
			Permission = permission;
		}
	}
}
