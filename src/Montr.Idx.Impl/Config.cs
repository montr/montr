using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.Models;

namespace Montr.Idx.Impl
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
		public static IEnumerable<Client> GetClients(ICollection<string> clientUrls)
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

					RedirectUris = clientUrls.Select(x => x + "/signin-oidc").ToList(),
					PostLogoutRedirectUris = clientUrls.Select(x => x + "/signout-callback-oidc").ToList(),
					AllowedCorsOrigins = clientUrls,

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
					// AccessTokenLifetime = 360, // seconds
					// IdentityTokenLifetime = 360, // seconds

					RedirectUris = clientUrls.SelectMany(x => new[] { x + "/signin-oidc", x + "/silent-renew-oidc" } ).ToList(),
					PostLogoutRedirectUris = clientUrls.Select(x => x + "/signout-callback-oidc").ToList(),
					AllowedCorsOrigins = clientUrls,

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
