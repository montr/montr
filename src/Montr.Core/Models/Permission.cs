using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Montr.Core.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Permission
	{
		private string DebuggerDisplay => $"{Code}";

		public const string ClaimType = nameof(Permission);

		public string Code { get; }

		public static string GetCode(Type permission)
		{
			return permission.FullName;
		}

		public Permission(Type permission) : this(GetCode(permission))
		{
		}

		public Permission(string code)
		{
			Code = code;
		}
	}

	public class RoleTemplate
	{
		public string RoleCode { get; set; }

		public IEnumerable<Permission> Permissions { get; set; }
	}
}
