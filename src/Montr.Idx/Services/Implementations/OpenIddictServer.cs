using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Montr.Idx.Commands.Oidc;
using Montr.Idx.Entities;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace Montr.Idx.Services.Implementations
{
	public class OpenIddictServer : IOidcServer
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IOpenIddictScopeManager _scopeManager;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly UserManager<DbUser> _userManager;
		private readonly IOptionsMonitor<IdentityOptions> _identityOptions;

		public OpenIddictServer(
			IHttpContextAccessor httpContextAccessor,
			IOpenIddictScopeManager scopeManager,
			SignInManager<DbUser> signInManager,
			UserManager<DbUser> userManager,
			IOptionsMonitor<IdentityOptions> identityOptions)
		{
			_httpContextAccessor = httpContextAccessor;
			_scopeManager = scopeManager;
			_signInManager = signInManager;
			_userManager = userManager;
			_identityOptions = identityOptions;
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
            var resources = await _scopeManager.ListResourcesAsync(scopes, cancellationToken).ToListAsync();

            principal.SetScopes(scopes);
            principal.SetResources(resources);

            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, principal));
            }

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return new SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);
		}

		public async Task<IActionResult> UserInfo(OidcUserInfo request, CancellationToken cancellationToken)
		{
			var httpContext = _httpContextAccessor.HttpContext
			                  ?? throw new InvalidOperationException("The HttpContext cannot be retrieved.");

			var principal = httpContext.User;

			var user = await _userManager.GetUserAsync(principal);

			if (user == null)
			{
				return new ChallengeResult(
					OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
					new AuthenticationProperties(new Dictionary<string, string>
					{
						[OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
						[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
							"The specified access token is bound to an account that no longer exists."
					}));
			}

			var claims = new Dictionary<string, object>(StringComparer.Ordinal)
			{
				// Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
				[Claims.Subject] = await _userManager.GetUserIdAsync(user)
			};

			if (principal.HasScope(Scopes.Email))
			{
				claims[Claims.Email] = await _userManager.GetEmailAsync(user);
				claims[Claims.EmailVerified] = await _userManager.IsEmailConfirmedAsync(user);
			}

			if (principal.HasScope(Scopes.Phone))
			{
				claims[Claims.PhoneNumber] = await _userManager.GetPhoneNumberAsync(user);
				claims[Claims.PhoneNumberVerified] = await _userManager.IsPhoneNumberConfirmedAsync(user);
			}

			if (principal.HasScope(Scopes.Roles))
			{
				claims[Claims.Role] = await _userManager.GetRolesAsync(user);
			}

			// Note: the complete list of standard claims supported by the OpenID Connect specification
			// can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

			return new OkObjectResult(claims);
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

		private IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
		{
			var identityOptions = _identityOptions.CurrentValue;

			// Never include the security stamp in the access and identity tokens, as it's a secret value.
			// "AspNet.Identity.SecurityStamp"
			if (claim.Type == identityOptions.ClaimsIdentity.SecurityStampClaimType)
			{
				yield break;
			}

			// Note: by default, claims are NOT automatically included in the access and identity tokens.
			// To allow OpenIddict to serialize them, you must attach them a destination, that specifies
			// whether they should be included in access tokens, in identity tokens or in both.

			switch (claim.Type)
			{
				case Claims.Name:
					yield return Destinations.AccessToken;

					if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
						yield return Destinations.IdentityToken;

					yield break;

				case Claims.Email:
					yield return Destinations.AccessToken;

					if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Email))
						yield return Destinations.IdentityToken;

					yield break;

				case Claims.Role:
					yield return Destinations.AccessToken;

					if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Roles))
						yield return Destinations.IdentityToken;

					yield break;

				default:
					yield return Destinations.AccessToken;

					yield break;
			}
		}
	}

	public static class AsyncEnumerableExtensions
	{
		public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			return ExecuteAsync();

			async Task<List<T>> ExecuteAsync()
			{
				var list = new List<T>();

				await foreach (var element in source)
				{
					list.Add(element);
				}

				return list;
			}
		}
	}
}
