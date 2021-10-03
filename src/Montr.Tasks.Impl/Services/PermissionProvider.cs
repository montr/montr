using System.Collections.Generic;
using Montr.Core;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Tasks.Impl.Services
{
	public class PermissionProvider : IPermissionProvider
	{
		public IEnumerable<Permission> GetPermissions()
		{
			return new[]
			{
				new Permission(typeof(Permissions.ViewTasks)),
				new Permission(typeof(Permissions.ManageTasks))
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
