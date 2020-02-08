using System;
using System.IdentityModel.Tokens.Jwt;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Idx.Impl.Services
{
	public static class OpenIdAuthServiceCollectionExtensions
	{
		public static AuthenticationBuilder AddOpenIdApiAuthentication(this IServiceCollection services, OpenIdOptions openIdOptions)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (openIdOptions == null) throw new ArgumentNullException(nameof(openIdOptions));

			// for api login
			return services
				.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
				.AddIdentityServerAuthentication(options =>
				{
					options.Authority = openIdOptions.Authority;
					options.RequireHttpsMetadata = false;

					options.ApiName = "tendr";
				});

			// https://github.com/IdentityServer/IdentityServer3/issues/487
			// leastprivilege commented on Nov 4, 2014
			// Just in general - i would recommend separating web app and api
			// - that way you don't run into the whole cookie vs token isolation issue and the related configuration complexity.

			// https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2836
			// Support both openId and bearer token of the identity server？
		}

		public static AuthenticationBuilder AddOpenIdAuthentication(this IServiceCollection services, OpenIdOptions openIdOptions)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (openIdOptions == null) throw new ArgumentNullException(nameof(openIdOptions));

			// for user login
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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

					// options.Scope.Add("tendr");
					options.Scope.Add("offline_access");
					// options.ClaimActions.MapJsonKey("website", "website");
				});
		}
	}
}
