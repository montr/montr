using Montr.Core.Models;

namespace Montr.Core.Services
{
	// todo: remove?
	public interface IPermissionResolver
	{
		Permission Resolve(string permissionName);
	}
}
