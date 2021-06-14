using System;
using System.Collections.Generic;

namespace Montr.Core.Models
{
	public class Permission
	{
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
