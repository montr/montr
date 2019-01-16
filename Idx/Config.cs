using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Idx
{
	public class Config
	{
		// scopes define the resources in your system
		public static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new List<IdentityResource>
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
				new IdentityResources.Address(),
				new IdentityResources.Email(),
				new IdentityResources.Phone()
			};
		}

		public static IEnumerable<ApiResource> GetApiResources()
		{
			return new List<ApiResource>
			{
				new ApiResource("tendr", "Tendr"),
			};
		}

		// clients want to access resources (aka scopes)
		public static IEnumerable<Client> GetClients()
		{
			return new List<Client>
			{
				// OpenID Connect hybrid flow and client credentials client
				new Client
				{
					ClientId = "tendr",
					ClientName = "Tendr",
					AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

					RequireConsent = false, // https://identityserver4.readthedocs.io/en/dev/quickstarts/6_aspnet_identity.html

					ClientSecrets =
					{
						new Secret("secret".Sha256())
					},

					RedirectUris =
					{
						"http://tendr.montr.io:5000/signin-oidc",
						"http://app.tendr.montr.io:5000/signin-oidc"
					},
					PostLogoutRedirectUris =
					{
						"http://tendr.montr.io:5000/signout-callback-oidc",
						"http://app.tendr.montr.io:5000/signout-callback-oidc",
					},
					AllowedCorsOrigins = { "http://app.tendr.montr.io:5000" },

					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"tendr"
					},
					AllowOfflineAccess = true
				},
				new Client
				{
					ClientId = "kompany",
					ClientName = "Kompany",
					AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

					RequireConsent = false,

					ClientSecrets =
					{
						new Secret("secret".Sha256())
					},

					RedirectUris = { "http://kompany.montr.io:5010/signin-oidc" },
					PostLogoutRedirectUris = { "http://kompany.montr.io:5010/signout-callback-oidc" },
					AllowedCorsOrigins = { "http://kompany.montr.io:5010" },

					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"tendr"
					},

					AllowOfflineAccess = true
				},
				new Client
				{
					ClientId = "ui",
					ClientName = "UI Client",
					// AllowedGrantTypes = GrantTypes.Implicit,
					AllowedGrantTypes = GrantTypes.Code,
					AllowAccessTokensViaBrowser = true,
					AlwaysIncludeUserClaimsInIdToken = true,
					RequireConsent = false,
					RequirePkce = true,
					RequireClientSecret = false,

					AllowOfflineAccess = true, // This feature refresh token
					AccessTokenLifetime = 360, // seconds
					IdentityTokenLifetime = 360, // seconds

					RedirectUris =
					{
						"http://kompany.montr.io:5010/signin-oidc",
						"http://kompany.montr.io:5010/silent-renew-oidc",
						"http://tendr.montr.io:5000/signin-oidc",
						"http://tendr.montr.io:5000/silent-renew-oidc",
						"http://app.tendr.montr.io:5000/signin-oidc",
						"http://app.tendr.montr.io:5000/silent-renew-oidc",
					},

					PostLogoutRedirectUris =
					{
						"http://kompany.montr.io:5010/signout-callback-oidc",
						"http://tendr.montr.io:5000/signout-callback-oidc",
						"http://app.tendr.montr.io:5000/signout-callback-oidc",
					},

					AllowedCorsOrigins = {
						"http://kompany.montr.io:5010",
						"http://tendr.montr.io:5000",
						"http://app.tendr.montr.io:5000",
					},

					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						IdentityServerConstants.StandardScopes.Address,
						IdentityServerConstants.StandardScopes.Email,
						IdentityServerConstants.StandardScopes.Phone,
						"tendr"
					}
				}
			};
		}
	}
}
