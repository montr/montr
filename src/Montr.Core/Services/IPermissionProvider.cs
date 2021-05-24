using System.Collections.Generic;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IPermissionProvider
	{
		/// <summary>
		/// Get all permissions available from this provider.
		/// </summary>
		/// <returns></returns>
		IEnumerable<Permission> GetPermissions();

		/// <summary>
		/// Get default role templates with permissions.
		/// </summary>
		/// <returns></returns>
		IEnumerable<RoleTemplate> GetRoleTemplates();
	}
}
