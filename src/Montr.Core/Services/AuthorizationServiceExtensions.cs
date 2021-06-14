using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public static class AuthorizationServiceExtensions
	{
		public static async Task<AuthorizationResult> AuthorizePermission(
			this IAuthorizationService authorizationService, ClaimsPrincipal principal, string permission)
		{
			return await authorizationService.AuthorizeAsync(principal, null, new[] { new PermissionRequirement(permission) });
		}
	}
}
