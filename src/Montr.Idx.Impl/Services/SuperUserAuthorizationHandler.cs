using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Montr.Core;
using Montr.Core.Models;
using OpenIddict.Abstractions;

namespace Montr.Idx.Impl.Services
{
	public class SuperUserAuthorizationHandler : IAuthorizationHandler
	{
		private readonly IOptionsMonitor<AppOptions> _optionsMonitor;

		public SuperUserAuthorizationHandler(IOptionsMonitor<AppOptions> optionsMonitor)
		{
			_optionsMonitor = optionsMonitor;
		}

		public Task HandleAsync(AuthorizationHandlerContext context)
		{
			var userId = context?.User.Claims.FirstOrDefault(x => x.Type == OpenIddictConstants.Claims.Subject)?.Value;

			if (userId == null) return Task.CompletedTask;

			var appOptions = _optionsMonitor.CurrentValue;

			if (appOptions.SuperUserId.HasValue && string.Equals(userId,
				appOptions.SuperUserId.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				foreach (var requirement in context.Requirements.OfType<PermissionRequirement>())
				{
					context.Succeed(requirement);
				}
			}

			return Task.CompletedTask;
		}
	}
}
