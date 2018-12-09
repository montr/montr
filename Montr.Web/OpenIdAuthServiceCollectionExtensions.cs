using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Web
{
	public class OpenIdOptions
	{
		public string Authority { get; set; }

		public string ClientId { get; set; }

		public string ClientSecret { get; set; }
	}

	public static class OpenIdAuthServiceCollectionExtensions
	{
		public static AuthenticationBuilder AddOpenIdAuthentication(this IServiceCollection services, OpenIdOptions openIdOptions)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (openIdOptions == null) throw new ArgumentNullException(nameof(openIdOptions));

			// for api login
			/*services
				.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
				.AddIdentityServerAuthentication(options =>
				{
					options.Authority = "http://idx.local:5050";
					options.RequireHttpsMetadata = false;

					options.ApiName = "tendr";
				});*/

			// https://github.com/IdentityServer/IdentityServer3/issues/487
			// leastprivilege commented on Nov 4, 2014
			// Just in general - i would recommend separating web app and api
			// - that way you don't run into the whole cookie vs token isolation issue and the related configuration complexity.

			// https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2836
			// Support both openId and bearer token of the identity server？

			// for user login
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

			// OpenIdOptions openIdOptions = configuration.GetSection("OpenId").Get<OpenIdOptions>();

			return services
				.AddAuthentication(options =>
				{
					options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
				})
				.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
				{
					options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

					options.Authority = openIdOptions.Authority;
					options.RequireHttpsMetadata = false;

					options.ClientId = openIdOptions.ClientId;
					options.ClientSecret = openIdOptions.ClientSecret;
					options.ResponseType = "code id_token"; // code id_token token

					options.SaveTokens = true;
					options.GetClaimsFromUserInfoEndpoint = true;

					options.Scope.Add("tendr");
					options.Scope.Add("offline_access");
					// options.ClaimActions.MapJsonKey("website", "website");
				});
		}
	}
}
