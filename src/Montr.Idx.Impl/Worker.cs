using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Montr.Idx.Impl.Entities;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Montr.Idx.Impl
{
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

			if (await manager.FindByClientIdAsync("ui", cancellationToken) is null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = "ui",
					// ConsentType = ConsentTypes.Explicit,
					ConsentType = ConsentTypes.Implicit,
					DisplayName = "UI Client",
					Type = ClientTypes.Public,
					PostLogoutRedirectUris =
					{
						new Uri("https://app.montr.io:5001/authentication/logout-callback"),
						new Uri("https://app.montr.io:5001/signout-callback-oidc"),
					},
					RedirectUris =
					{
						new Uri("https://app.montr.io:5001/authentication/login-callback"),
						new Uri("https://app.montr.io:5001/signin-oidc"),
						new Uri("https://app.montr.io:5001/silent-renew-oidc"),
					},
					Permissions =
					{
						Permissions.Endpoints.Authorization,
						Permissions.Endpoints.Logout,
						Permissions.Endpoints.Token,

						Permissions.GrantTypes.Implicit,
						Permissions.GrantTypes.AuthorizationCode,
						Permissions.GrantTypes.RefreshToken,

						Permissions.ResponseTypes.Code,
						Permissions.ResponseTypes.IdToken,
						Permissions.ResponseTypes.IdTokenToken,
						Permissions.ResponseTypes.Token,

						Permissions.Scopes.Email,
						Permissions.Scopes.Profile,
						Permissions.Scopes.Roles
					},
					/*Requirements =
					{
						Requirements.Features.ProofKeyForCodeExchange
					}*/
				});
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
