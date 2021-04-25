using System;

namespace Montr.Core.Models
{
	public static class PermissionPolicy
	{
		private const string Prefix = nameof(Permission) + ":";

		public static string BuildPolicyName(Type permission)
		{
			return Prefix + Permission.GetName(permission);
		}

		public static bool TryGetPermissionName(string policyName, out string permissionName)
		{
			if (policyName?.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase) == true)
			{
				permissionName = policyName[Prefix.Length..];
				return true;
			}

			permissionName = null;
			return false;
		}
	}
}
