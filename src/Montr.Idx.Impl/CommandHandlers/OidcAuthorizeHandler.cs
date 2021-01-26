using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB.Async;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Montr.Idx.Impl.CommandHandlers
{
	// from Balosar.Server.Controllers.AuthorizationController.Authorize()
	public class OidcAuthorizeHandler : IRequestHandler<OidcAuthorize, IActionResult>
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IOpenIddictApplicationManager _applicationManager;
		private readonly IOpenIddictAuthorizationManager _authorizationManager;
		private readonly IOpenIddictScopeManager _scopeManager;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly UserManager<DbUser> _userManager;

		public OidcAuthorizeHandler(
			IHttpContextAccessor httpContextAccessor,
			IOpenIddictApplicationManager applicationManager,
			IOpenIddictAuthorizationManager authorizationManager,
			IOpenIddictScopeManager scopeManager,
			SignInManager<DbUser> signInManager,
			UserManager<DbUser> userManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_applicationManager = applicationManager;
			_authorizationManager = authorizationManager;
			_scopeManager = scopeManager;
			_signInManager = signInManager;
			_userManager = userManager;
		}

		public async Task<IActionResult> Handle(OidcAuthorize request, CancellationToken cancellationToken)
		{
			var httpContext = _httpContextAccessor.HttpContext
				?? throw new InvalidOperationException("The HttpContext cannot be retrieved.");

			var httpRequest = httpContext.Request;

			var oidcRequest = httpContext.GetOpenIddictServerRequest()
			              ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

			var controller = request.Controller;

            if (!controller.User.Identity.IsAuthenticated)
            {
                // If the client application request promptless authentication,
                // return an error indicating that the user is not logged in.
                if (oidcRequest.HasPrompt(OpenIddictConstants.Prompts.None))
                {
                    var properties = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.LoginRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The user is not logged in."
                    });

                    // Ask OpenIddict to return a login_required error to the client application.
                    return controller.Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                return controller.Challenge();
            }

            // Retrieve the profile of the logged in user.
            var user = await _userManager.GetUserAsync(controller.User) ??
                throw new InvalidOperationException("The user details cannot be retrieved.");

            // Create a new ClaimsPrincipal containing the claims that
            // will be used to create an id_token, a token or a code.
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            // Set the list of scopes granted to the client application.
            var scopes = oidcRequest.GetScopes();

            principal.SetScopes(oidcRequest.GetScopes());
            principal.SetResources(await _scopeManager.ListResourcesAsync(scopes).ToListAsync());

            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, principal));
            }

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return controller.SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
		}

		private IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
		{
			// Note: by default, claims are NOT automatically included in the access and identity tokens.
			// To allow OpenIddict to serialize them, you must attach them a destination, that specifies
			// whether they should be included in access tokens, in identity tokens or in both.

			switch (claim.Type)
			{
				case OpenIddictConstants.Claims.Name:
					yield return OpenIddictConstants.Destinations.AccessToken;

					if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
						yield return OpenIddictConstants.Destinations.IdentityToken;

					yield break;

				case OpenIddictConstants.Claims.Email:
					yield return OpenIddictConstants.Destinations.AccessToken;

					if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Email))
						yield return OpenIddictConstants.Destinations.IdentityToken;

					yield break;

				case OpenIddictConstants.Claims.Role:
					yield return OpenIddictConstants.Destinations.AccessToken;

					if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Roles))
						yield return OpenIddictConstants.Destinations.IdentityToken;

					yield break;

				// Never include the security stamp in the access and identity tokens, as it's a secret value.
				case "AspNet.Identity.SecurityStamp": yield break;

				default:
					yield return OpenIddictConstants.Destinations.AccessToken;
					yield break;
			}
		}
	}
}
