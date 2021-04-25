using System.Collections.Generic;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IPermissionProvider
	{
		IEnumerable<Permission> GetPermissions();
	}
}
