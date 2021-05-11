using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Montr.Core.Models;

namespace Montr.Idx.Impl.Services
{
	public class UserPermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
		{
			if (context.HasSucceeded)
			{
				return Task.CompletedTask;
			}

			if (context.User.Identity?.IsAuthenticated == true &&
			    context.User.HasClaim(Permission.ClaimType, requirement.Permission))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
