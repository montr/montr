using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IPermissionResolver
	{
		Permission Resolve(string permissionName);
	}
}
