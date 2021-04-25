using System;
using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public class DefaultPermissionResolver : IPermissionResolver
	{
		private readonly IDictionary<string, Permission> _permissionByName =
			new Dictionary<string, Permission>(StringComparer.OrdinalIgnoreCase);

		public DefaultPermissionResolver(IEnumerable<IPermissionProvider> permissionProviders)
		{
			foreach (var provider in permissionProviders)
			{
				foreach (var permission in provider.GetPermissions())
				{
					_permissionByName[permission.Name] = permission;
				}
			}
		}

		public Permission Resolve(string permissionName)
		{
			if (_permissionByName.TryGetValue(permissionName, out var result))
			{
				return result;
			}

			throw new InvalidOperationException(
				$"Permission {permissionName} is not found. Check that it is provided by one of IPermissionProvider.");
		}
	}
}
