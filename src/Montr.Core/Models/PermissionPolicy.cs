using System;

namespace Montr.Core.Models
{
	public static class PermissionPolicy
	{
		private const string Prefix = nameof(Permission) + ":";

		public static string BuildPolicyName(Type permission)
		{
			return Prefix + Permission.GetCode(permission);
		}

		public static bool TryGetPermission(string policyName, out string permission)
		{
			if (policyName?.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase) == true)
			{
				permission = policyName[Prefix.Length..];
				return true;
			}

			permission = null;
			return false;
		}
	}
}
