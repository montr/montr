using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.MasterData.Impl.Services
{
	public class PermissionProvider : IPermissionProvider
	{
		public IEnumerable<Permission> GetPermissions()
		{
			return new[]
			{
				new Permission(typeof(Queries.GetClassifierTypeList)),
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
