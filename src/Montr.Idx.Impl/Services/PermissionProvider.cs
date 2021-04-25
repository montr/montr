using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Idx.Impl.Services
{
	public class PermissionProvider : IPermissionProvider
	{
		public IEnumerable<Permission> GetPermissions()
		{
			return new[]
			{
				new Permission(typeof(Permissions.ViewUserRoles)),
				new Permission(typeof(Permissions.ManageUserRoles))
			};
		}
	}
}
