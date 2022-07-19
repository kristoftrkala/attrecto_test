using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Attrecto.IdentityServer
{
    public class IdentityConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

        public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "client",

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                RedirectUris =
                {
                    "https://localhost:7015/signin-oidc",
                    "http://localhost:7015/signin-oidc"
                },

                // secret for authentication
                ClientSecrets =
                {
                    new Secret("kgflsdadjsadnfad43snfdsak?j#fdsa".Sha256())
                },

                // scopes that client has access to
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "attrecto_api"
                },
                AllowOfflineAccess = true,
                AbsoluteRefreshTokenLifetime = 31536000,
                SlidingRefreshTokenLifetime = 31536000,
                RefreshTokenExpiration = TokenExpiration.Sliding
            }
        };

        public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("attrecto_api", "Attrecto API")
            {
                UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Email, "role" },
                Scopes = new List<string>{ "attrecto_api" },
            }
        };

        public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope
            {
                Name = "attrecto_api"
            }
        };
    }
}
