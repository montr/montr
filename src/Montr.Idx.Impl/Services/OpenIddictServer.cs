using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB.Async;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Montr.Idx.Commands.Oidc;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Services;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Montr.Idx.Impl.Services
{
	public class OpenIddictServer : IOidcServer
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IOpenIddictApplicationManager _applicationManager;
		private readonly IOpenIddictAuthorizationManager _authorizationManager;
		private readonly IOpenIddictScopeManager _scopeManager;
		private readonly Microsoft.AspNetCore.Identity.SignInManager<DbUser> _signInManager;
		private readonly Microsoft.AspNetCore.Identity.UserManager<DbUser> _userManager;

		public OpenIddictServer(
			IHttpContextAccessor httpContextAccessor,
			IOpenIddictApplicationManager applicationManager,
			IOpenIddictAuthorizationManager authorizationManager,
			IOpenIddictScopeManager scopeManager,
			Microsoft.AspNetCore.Identity.SignInManager<DbUser> signInManager,
			Microsoft.AspNetCore.Identity.UserManager<DbUser> userManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_applicationManager = applicationManager;
			_authorizationManager = authorizationManager;
			_scopeManager = scopeManager;
			_signInManager = signInManager;
			_userManager = userManager;
		}

		public async Task<IActionResult> Authorize(OidcAuthorize request, CancellationToken cancellationToken)
		{
			var httpContext = _httpContextAccessor.HttpContext
			                  ?? throw new InvalidOperationException("The HttpContext cannot be retrieved.");

			var oidcRequest = httpContext.GetOpenIddictServerRequest()
			                  ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

			if (oidcRequest.IsImplicitFlow())
			{
				return await AuthorizeImplicitFlow(oidcRequest, httpContext, cancellationToken);
			}

			throw new InvalidOperationException($"Response type \"{oidcRequest.ResponseType}\" is not supported.");
		}

		private async Task<IActionResult> AuthorizeImplicitFlow(
			OpenIddictRequest oidcRequest, HttpContext httpContext, CancellationToken cancellationToken)
		{
			if (httpContext.User.Identity?.IsAuthenticated == false)
            {
                // If the client application request promptless authentication,
                // return an error indicating that the user is not logged in.
                if (oidcRequest.HasPrompt(Prompts.None))
                {
                    var properties = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The user is not logged in."
                    });

                    // Ask OpenIddict to return a login_required error to the client application.
                    return new ForbidResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, properties);
                }

                return new ChallengeResult();
            }

            // Retrieve the profile of the logged in user.
            var dbUser = await _userManager.GetUserAsync(httpContext.User) ??
                throw new InvalidOperationException("The user details cannot be retrieved.");

            // Create a new ClaimsPrincipal containing the claims that
            // will be used to create an id_token, a token or a code.
            var principal = await _signInManager.CreateUserPrincipalAsync(dbUser);

            // Set the list of scopes granted to the client application.
            var scopes = oidcRequest.GetScopes();
            var resources = await _scopeManager.ListResourcesAsync(scopes, cancellationToken).ToListAsync(cancellationToken);

            principal.SetScopes(scopes);
            principal.SetResources(resources);

            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, principal));
            }

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return new SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);
		}

		public async Task<IActionResult> Logout(OidcLogout request, CancellationToken cancellationToken)
		{
			// Ask ASP.NET Core Identity to delete the local and external cookies created
			// when the user agent is redirected from the external identity provider
			// after a successful authentication flow (e.g Google or Facebook).
			await _signInManager.SignOutAsync();

			// Returning a SignOutResult will ask OpenIddict to redirect the user agent
			// to the post_logout_redirect_uri specified by the client application.
			return new SignOutResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
		}

		private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
		{
			// Note: by default, claims are NOT automatically included in the access and identity tokens.
			// To allow OpenIddict to serialize them, you must attach them a destination, that specifies
			// whether they should be included in access tokens, in identity tokens or in both.

			switch (claim.Type)
			{
				case Claims.Name:
					yield return Destinations.AccessToken;

					if (principal.HasScope(Permissions.Scopes.Profile))
						yield return Destinations.IdentityToken;

					yield break;

				case Claims.Email:
					yield return Destinations.AccessToken;

					if (principal.HasScope(Permissions.Scopes.Email))
						yield return Destinations.IdentityToken;

					yield break;

				case Claims.Role:
					yield return Destinations.AccessToken;

					if (principal.HasScope(Permissions.Scopes.Roles))
						yield return Destinations.IdentityToken;

					yield break;

				// Never include the security stamp in the access and identity tokens, as it's a secret value.
				case "AspNet.Identity.SecurityStamp": yield break;

				default:
					yield return Destinations.AccessToken;
					yield break;
			}
		}
	}
}
