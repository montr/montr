using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Montr.Idx.Entities;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Montr.Idx.Services.Implementations
{
	// todo: remove after implementing ui for oidc clients
	public class Worker : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;

		public Worker(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = _serviceProvider.CreateScope();

			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			await context.Database.EnsureCreatedAsync(cancellationToken);

			var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

			var clientId = "ui";
			var clientName = "UI Client";
			var baseUrl = "https://127.0.0.1:5001";

			if (await manager.FindByClientIdAsync(clientId, cancellationToken) is null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = clientId,
					// ConsentType = ConsentTypes.Explicit,
					ConsentType = ConsentTypes.Implicit,
					DisplayName = clientName,
					DisplayNames = {  },
					Type = ClientTypes.Public,
					PostLogoutRedirectUris =
					{
						new Uri($"{baseUrl}/authentication/logout-callback"),
						new Uri($"{baseUrl}/signout-callback-oidc"),
					},
					RedirectUris =
					{
						new Uri($"{baseUrl}/authentication/login-callback"),
						new Uri($"{baseUrl}/signin-oidc"),
						new Uri($"{baseUrl}/silent-renew-oidc"),
					},
					Permissions =
					{
						OpenIddictConstants.Permissions.Endpoints.Authorization,
						OpenIddictConstants.Permissions.Endpoints.Token,
						OpenIddictConstants.Permissions.Endpoints.Logout,

						OpenIddictConstants.Permissions.GrantTypes.Implicit,
						OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
						OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

						OpenIddictConstants.Permissions.ResponseTypes.Code,
						OpenIddictConstants.Permissions.ResponseTypes.IdToken,
						OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken,
						OpenIddictConstants.Permissions.ResponseTypes.Token,

						OpenIddictConstants.Permissions.Scopes.Email,
						OpenIddictConstants.Permissions.Scopes.Profile,
						OpenIddictConstants.Permissions.Scopes.Roles
					},
					/*Requirements =
					{
						Requirements.Features.ProofKeyForCodeExchange
					}*/
				}, cancellationToken);
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
