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
                new IdentityResources.Profile()
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
            // client credentials client
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

                    RedirectUris = { "http://tendr.local:5000/signin-oidc", "http://app.tendr.local:5000/signin-oidc" },
                    PostLogoutRedirectUris = { "http://app.tendr.local:5000/signout-callback-oidc" },

                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "tendr"
                    },
                    AllowOfflineAccess = true
                }
            };
        }
    }
}
