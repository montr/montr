﻿using System.Collections.Generic;
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
				new Permission(typeof(Permissions.ManageUserRoles)),

				new Permission(typeof(Permissions.ViewRolePermissions)),
				new Permission(typeof(Permissions.ManageRolePermissions))
			};
		}

		public IEnumerable<RoleTemplate> GetRoleTemplates()
		{
			return new[]
			{
				new RoleTemplate
				{
					RoleCode = "Administrator",
					Permissions = GetPermissions()
				}
			};
		}
	}
}
