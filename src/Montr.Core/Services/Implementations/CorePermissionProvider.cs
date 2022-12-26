using System.Collections.Generic;
using Montr.Core.Models;

namespace Montr.Core.Services.Implementations
{
	public class CorePermissionProvider : IPermissionProvider
	{
		public IEnumerable<Permission> GetPermissions()
		{
			return new[]
			{
				new Permission(typeof(Permissions.ViewLocales)),
				new Permission(typeof(Permissions.ManageLocales))
			};
		}

		public IEnumerable<RoleTemplate> GetRoleTemplates()
		{
			return new[]
			{
				new RoleTemplate
				{
					RoleCode = DefaultRoleCode.Administrator,
					Permissions = GetPermissions()
				}
			};
		}
	}
}
