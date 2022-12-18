using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Montr.Idx.Commands.Oidc;
using Montr.Idx.Entities;
using Montr.Idx.Models;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace Montr.Idx.Services.Implementations
{
	public class OpenIddictServer : IOidcServer
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IOpenIddictApplicationManager _applicationManager;
		private readonly IOpenIddictAuthorizationManager _authorizationManager;
		private readonly IOpenIddictScopeManager _scopeManager;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly UserManager<DbUser> _userManager;
		private readonly IOptionsMonitor<IdentityOptions> _identityOptions;

		public OpenIddictServer(
			IHttpContextAccessor httpContextAccessor,
			IOpenIddictApplicationManager applicationManager,
			IOpenIddictAuthorizationManager authorizationManager,
			IOpenIddictScopeManager scopeManager,
			SignInManager<DbUser> signInManager,
			UserManager<DbUser> userManager,
			IOptionsMonitor<IdentityOptions> identityOptions)
		{
			_httpContextAccessor = httpContextAccessor;
			_applicationManager = applicationManager;
			_authorizationManager = authorizationManager;
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

			if (oidcRequest.IsAuthorizationCodeFlow())
			{
				return await AuthorizeCodeFlow(oidcRequest, httpContext, cancellationToken);
			}

			if (oidcRequest.IsImplicitFlow())
			{
				return await AuthorizeImplicitFlow(oidcRequest, httpContext, cancellationToken);
			}

			throw new InvalidOperationException($"Response type \"{oidcRequest.ResponseType}\" is not supported.");
		}

		// Base on https://github.com/openiddict/openiddict-samples/blob/dev/samples/Velusia/Velusia.Server/Controllers/AuthorizationController.cs
		private async Task<IActionResult> AuthorizeCodeFlow(
			OpenIddictRequest oidcRequest, HttpContext httpContext, CancellationToken cancellationToken)
		{
			// Try to retrieve the user principal stored in the authentication cookie and redirect
			// the user agent to the login page (or to an external provider) in the following cases:
			//
			//  - If the user principal can't be extracted or the cookie is too old.
			//  - If prompt=login was specified by the client application.
			//  - If a max_age parameter was provided and the authentication cookie is not considered "fresh" enough.
			var result = await httpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

			if (result.Succeeded == false || oidcRequest.HasPrompt(Prompts.Login) ||
			    (oidcRequest.MaxAge != null && result.Properties?.IssuedUtc != null &&
			     DateTimeOffset.UtcNow - result.Properties.IssuedUtc > TimeSpan.FromSeconds(oidcRequest.MaxAge.Value)))
			{
				// If the client application requested promptless authentication,
				// return an error indicating that the user is not logged in.
				if (oidcRequest.HasPrompt(Prompts.None))
				{
					return new ForbidResult(
						authenticationSchemes: new[] {OpenIddictServerAspNetCoreDefaults.AuthenticationScheme},
						properties: new AuthenticationProperties(new Dictionary<string, string>
						{
							[OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
							[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
								"The user is not logged in."
						}));
				}

				// To avoid endless login -> authorization redirects, the prompt=login flag
				// is removed from the authorization request payload before redirecting the user.
				var prompt = string.Join(" ", oidcRequest.GetPrompts().Remove(Prompts.Login));

				var parameters = httpContext.Request.HasFormContentType
					? httpContext.Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList()
					: httpContext.Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

				parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));

				return new ChallengeResult(
					authenticationSchemes: new[] {IdentityConstants.ApplicationScheme},
					properties: new AuthenticationProperties
					{
						RedirectUri = httpContext.Request.PathBase + httpContext.Request.Path +
						              QueryString.Create(parameters)
					});
			}

			// Retrieve the profile of the logged in user.
			var user = await _userManager.GetUserAsync(result.Principal) ??
			           throw new InvalidOperationException("The user details cannot be retrieved.");

			// Retrieve the application details from the database.
			var application = await _applicationManager.FindByClientIdAsync(oidcRequest.ClientId, cancellationToken) ??
			                  throw new InvalidOperationException(
				                  "Details concerning the calling client application cannot be found.");

			// Retrieve the permanent authorizations associated with the user and the calling client application.
			var authorizations = await _authorizationManager.FindAsync(
				subject: await _userManager.GetUserIdAsync(user),
				client: await _applicationManager.GetIdAsync(application, cancellationToken),
				status: Statuses.Valid,
				type: AuthorizationTypes.Permanent,
				scopes: oidcRequest.GetScopes(), cancellationToken).ToListAsync();

			switch (await _applicationManager.GetConsentTypeAsync(application, cancellationToken))
			{
				// If the consent is external (e.g when authorizations are granted by a sysadmin),
				// immediately return an error if no authorization can be found in the database.
				case ConsentTypes.External when !authorizations.Any():

					return new ForbidResult(
						authenticationSchemes: new[] {OpenIddictServerAspNetCoreDefaults.AuthenticationScheme},
						properties: new AuthenticationProperties(new Dictionary<string, string>
						{
							[OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
							[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
								"The logged in user is not allowed to access this client application."
						}));

				// If the consent is implicit or if an authorization was found,
				// return an authorization response without displaying the consent form.
				case ConsentTypes.Implicit:
				case ConsentTypes.External when authorizations.Any():
				case ConsentTypes.Explicit when authorizations.Any() && !oidcRequest.HasPrompt(Prompts.Consent):

					// Create the claims-based identity that will be used by OpenIddict to generate tokens.
					var identity = new ClaimsIdentity(
						authenticationType: TokenValidationParameters.DefaultAuthenticationType,
						nameType: Claims.Name,
						roleType: Claims.Role);

					// Add the claims that will be persisted in the tokens.
					identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
						.SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
						.SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
						.SetClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

					// Note: in this sample, the granted scopes match the requested scope
					// but you may want to allow the user to uncheck specific scopes.
					// For that, simply restrict the list of scopes before calling SetScopes.
					identity.SetScopes(oidcRequest.GetScopes());
					identity.SetResources(
						await _scopeManager.ListResourcesAsync(identity.GetScopes(), cancellationToken).ToListAsync());

					// Automatically create a permanent authorization to avoid requiring explicit consent
					// for future authorization or token requests containing the same scopes.
					var authorization = authorizations.LastOrDefault();
					authorization ??= await _authorizationManager.CreateAsync(
						identity: identity,
						subject: await _userManager.GetUserIdAsync(user),
						client: await _applicationManager.GetIdAsync(application, cancellationToken),
						type: AuthorizationTypes.Permanent,
						scopes: identity.GetScopes(), cancellationToken);

					identity.SetAuthorizationId(
						await _authorizationManager.GetIdAsync(authorization, cancellationToken));
					identity.SetDestinations(GetDestinations);

					return new SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
						new ClaimsPrincipal(identity));

				// At this point, no authorization was found in the database and an error must be returned
				// if the client application specified prompt=none in the authorization request.
				case ConsentTypes.Explicit when oidcRequest.HasPrompt(Prompts.None):
				case ConsentTypes.Systematic when oidcRequest.HasPrompt(Prompts.None):

					return new ForbidResult(
						authenticationSchemes: new[] {OpenIddictServerAspNetCoreDefaults.AuthenticationScheme},
						properties: new AuthenticationProperties(new Dictionary<string, string>
						{
							[OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
							[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
								"Interactive user consent is required."
						}));

				// In every other case, render the consent form.
				default:
					// return new ViewResult
					return new JsonResult(new AuthorizeViewModel
					{
						ApplicationName =
							await _applicationManager.GetLocalizedDisplayNameAsync(application, cancellationToken),
						Scope = oidcRequest.Scope
					});
			}
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

		public async Task<IActionResult> Token(OidcToken request, CancellationToken cancellationToken)
		{
			var httpContext = _httpContextAccessor.HttpContext
			                  ?? throw new InvalidOperationException("The HttpContext cannot be retrieved.");

			var oidcRequest = httpContext.GetOpenIddictServerRequest()
			                  ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

			if (oidcRequest.IsAuthorizationCodeGrantType() || oidcRequest.IsRefreshTokenGrantType())
			{
				// Retrieve the claims principal stored in the authorization code/refresh token.
				var result =
					await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

				// Retrieve the user profile corresponding to the authorization code/refresh token.
				var user = await _userManager.FindByIdAsync(result.Principal.GetClaim(Claims.Subject));
				if (user is null)
				{
					return new ForbidResult(
						authenticationSchemes: new[] {OpenIddictServerAspNetCoreDefaults.AuthenticationScheme},
						properties: new AuthenticationProperties(new Dictionary<string, string>
						{
							[OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
							[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
								"The token is no longer valid."
						}));
				}

				// Ensure the user is still allowed to sign in.
				if (!await _signInManager.CanSignInAsync(user))
				{
					return new ForbidResult(
						authenticationSchemes: new[] {OpenIddictServerAspNetCoreDefaults.AuthenticationScheme},
						properties: new AuthenticationProperties(new Dictionary<string, string>
						{
							[OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
							[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
								"The user is no longer allowed to sign in."
						}));
				}

				var identity = new ClaimsIdentity(result.Principal.Claims,
					authenticationType: TokenValidationParameters.DefaultAuthenticationType,
					nameType: Claims.Name,
					roleType: Claims.Role);

				// Override the user claims present in the principal in case they
				// changed since the authorization code/refresh token was issued.
				identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
					.SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
					.SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
					.SetClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

				identity.SetDestinations(GetDestinations);

				// Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
				return new SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
					new ClaimsPrincipal(identity));
			}

			throw new InvalidOperationException("The specified grant type is not supported.");
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

		// Used in code flow.
		// todo: merge with GetDestinations() from implicit flow
		private static IEnumerable<string> GetDestinations(Claim claim)
		{
			// Note: by default, claims are NOT automatically included in the access and identity tokens.
			// To allow OpenIddict to serialize them, you must attach them a destination, that specifies
			// whether they should be included in access tokens, in identity tokens or in both.

			switch (claim.Type)
			{
				case Claims.Name:
					yield return Destinations.AccessToken;

					if (claim.Subject.HasScope(Scopes.Profile))
						yield return Destinations.IdentityToken;

					yield break;

				case Claims.Email:
					yield return Destinations.AccessToken;

					if (claim.Subject.HasScope(Scopes.Email))
						yield return Destinations.IdentityToken;

					yield break;

				case Claims.Role:
					yield return Destinations.AccessToken;

					if (claim.Subject.HasScope(Scopes.Roles))
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
