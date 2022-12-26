﻿using System.Collections.Generic;
using Montr.Core;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Docs.Services.Implementations
{
	public class DocsPermissionProvider : IPermissionProvider
	{
		public IEnumerable<Permission> GetPermissions()
		{
			return new[]
			{
				new Permission(typeof(Permissions.ViewDocuments)),
				new Permission(typeof(Permissions.SubmitDocument))
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
				},
				new RoleTemplate
				{
					RoleCode = DefaultRoleCode.LimitedCompanyAdministrator,
					Permissions = new []
					{
						new Permission(typeof(Permissions.ViewDocuments)),
						new Permission(typeof(Permissions.SubmitDocument))
					}
				}
			};
		}
	}
}
