using System.Collections.Generic;
using Montr.Core;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.MasterData.Services.Implementations
{
	public class PermissionProvider : IPermissionProvider
	{
		public IEnumerable<Permission> GetPermissions()
		{
			return new[]
			{
				new Permission(typeof(Permissions.ViewClassifierTypes)),
				new Permission(typeof(Permissions.ManageClassifierTypes)),

				new Permission(typeof(Permissions.ViewClassifiers)),
				new Permission(typeof(Permissions.ManageClassifiers))
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
