using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Idx.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Idx.Controllers
{
	// https://github.com/IdentityServer/IdentityServer4.Samples/blob/master/Quickstarts/6_AspNetIdentity/src/IdentityServerWithAspNetIdentity/Quickstart/Account/AccountController.cs
	[SecurityHeaders]
	public class AccountController : Controller
	{
		private readonly UserManager<DbUser> _userManager;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly IIdentityServerInteractionService _interaction;
		private readonly IClientStore _clientStore;
		private readonly IAuthenticationSchemeProvider _schemeProvider;
		private readonly IEventService _events;

		public AccountController(
			UserManager<DbUser> userManager,
			SignInManager<DbUser> signInManager,
			IIdentityServerInteractionService interaction,
			IClientStore clientStore,
			IAuthenticationSchemeProvider schemeProvider,
			IEventService events)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_interaction = interaction;
			_clientStore = clientStore;
			_schemeProvider = schemeProvider;
			_events = events;
		}

		/// <summary>
		/// Post processing of external authentication
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> ExternalLoginCallback()
		{
			// read external identity from the temporary cookie
			var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
			if (result?.Succeeded != true)
			{
				throw new Exception("External authentication error");
			}

			// lookup our user and external provider info
			var (user, provider, providerUserId, claims) = await FindUserFromExternalProviderAsync(result);
			if (user == null)
			{
				// this might be where you might initiate a custom workflow for user registration
				// in this sample we don't show how that would be done, as our sample implementation
				// simply auto-provisions new external user
				user = await AutoProvisionUserAsync(provider, providerUserId, claims);
			}

			// this allows us to collect any additional claims or properties
			// for the specific protocols used and store them in the local auth cookie.
			// this is typically used to store data needed for signout from those protocols.
			var additionalLocalClaims = new List<Claim>();
			var localSignInProps = new AuthenticationProperties();
			ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);
			ProcessLoginCallbackForWsFed(result, additionalLocalClaims, localSignInProps);
			ProcessLoginCallbackForSaml2p(result, additionalLocalClaims, localSignInProps);

			// issue authentication cookie for user
			// we must issue the cookie manually, and can't use the SignInManager because
			// it doesn't expose an API to issue additional claims from the login workflow
			var principal = await _signInManager.CreateUserPrincipalAsync(user);
			additionalLocalClaims.AddRange(principal.Claims);
			var name = principal.FindFirst(JwtClaimTypes.Name)?.Value /*?? user.Id*/;
			await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id.ToString(), name));
			await HttpContext.SignInAsync(user.Id.ToString(), name, provider, localSignInProps, additionalLocalClaims.ToArray());

			// delete temporary cookie used during external authentication
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			// validate return URL and redirect back to authorization endpoint or a local page
			if (result.Properties.Items.TryGetValue("returnUrl", out string returnUrl))
			{
				if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
				{
					return Redirect(returnUrl);
				}
			}

			return Redirect("~/");
		}

		private async Task<(DbUser user, string provider, string providerUserId, IEnumerable<Claim> claims)> 
			FindUserFromExternalProviderAsync(AuthenticateResult result)
		{
			var externalUser = result.Principal;

			// try to determine the unique id of the external user (issued by the provider)
			// the most common claim type for that are the sub claim and the NameIdentifier
			// depending on the external provider, some other claim type might be used
			var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
							externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
							throw new Exception("Unknown userid");

			// remove the user id claim so we don't include it as an extra claim if/when we provision the user
			var claims = externalUser.Claims.ToList();
			claims.Remove(userIdClaim);

			// var provider = result.Properties.Items["scheme"];
			var provider = result.Properties.Items[".AuthScheme"];
			var providerUserId = userIdClaim.Value;

			// find external user
			var user = await _userManager.FindByLoginAsync(provider, providerUserId);

			return (user, provider, providerUserId, claims);
		}

		private async Task<DbUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
		{
			// create a list of claims that we want to transfer into our store
			var filtered = new List<Claim>();

			// user's display name
			var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
				claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
			if (name != null)
			{
				filtered.Add(new Claim(JwtClaimTypes.Name, name));
			}
			else
			{
				var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
					claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
				var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
					claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
				if (first != null && last != null)
				{
					filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
				}
				else if (first != null)
				{
					filtered.Add(new Claim(JwtClaimTypes.Name, first));
				}
				else if (last != null)
				{
					filtered.Add(new Claim(JwtClaimTypes.Name, last));
				}
			}

			// email
			var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
			   claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
			if (email != null)
			{
				filtered.Add(new Claim(JwtClaimTypes.Email, email));
			}

			DbUser user;
			IdentityResult identityResult;

			if (User.IsAuthenticated())
			{
				user = await _userManager.FindByNameAsync(User.Identity.Name);
			}
			else
			{
				user = new DbUser
				{
					UserName = Guid.NewGuid().ToString(),
				};

				identityResult = await _userManager.CreateAsync(user);
				if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

			}

			if (filtered.Any())
			{
				identityResult = await _userManager.AddClaimsAsync(user, filtered);
				if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
			}

			identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
			if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

			return user;
		}

		private void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
		{
			// if the external system sent a session id claim, copy it over
			// so we can use it for single sign-out
			var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
			if (sid != null)
			{
				localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
			}

			// if the external provider issued an id_token, we'll keep it for signout
			var id_token = externalResult.Properties.GetTokenValue("id_token");
			if (id_token != null)
			{
				localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
			}
		}

		private void ProcessLoginCallbackForWsFed(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
		{
		}

		private void ProcessLoginCallbackForSaml2p(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
		{
		}
	}

	public class SecurityHeadersAttribute : ActionFilterAttribute
	{
		public override void OnResultExecuting(ResultExecutingContext context)
		{
			var result = context.Result;
			if (result is ViewResult)
			{
				// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
				if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
				{
					context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
				}

				// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
				if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
				{
					context.HttpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
				}

				// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
				var csp = "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';";
				// also consider adding upgrade-insecure-requests once you have HTTPS in place for production
				//csp += "upgrade-insecure-requests;";
				// also an example if you need client images to be displayed from twitter
				// csp += "img-src 'self' https://pbs.twimg.com;";

				// once for standards compliant browsers
				if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
				{
					context.HttpContext.Response.Headers.Add("Content-Security-Policy", csp);
				}
				// and once again for IE
				if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
				{
					context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp);
				}

				// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
				var referrer_policy = "no-referrer";
				if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy"))
				{
					context.HttpContext.Response.Headers.Add("Referrer-Policy", referrer_policy);
				}
			}
		}
	}
}