using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace EduManagementLab.IdentityServer
{
    public static class TestData
    {
        public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource
            {
                Name = "role",
                UserClaims = new List<string> {"role"}
            }
        };
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
        {
            //used to specify what actions authorized user can perform at the level of the API
             new ApiScope("eduManagementLabApi.read", "Read Access to EduManagementLab API"),
             new ApiScope("eduManagementLabApi.write", "Write Access to EduManagementLab API"),
        };
        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                //OAuth2 
                ClientId = "eduManagementLabApi",
                ClientName = "ASP.NET Core EduManagementLab Api",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret> {new Secret("TestEduApi".Sha256())},
                AllowedScopes = new List<string> {"eduManagementLabApi.read"},
                AllowedCorsOrigins = new List<string>
                {
                    "https://localhost:7134",
                    "https://localhost:7243",
                    "https://localhost:7187"
                }
            },
            new Client
            {
                //OpenID Connect
                ClientId = "oidcEduWebApp",
                ClientName = "ASP.NET Core EduManagementLab Web",
                ClientSecrets =  {new Secret("TestEduApi".Sha256())},
                AllowedGrantTypes = GrantTypes.Code,
                AllowOfflineAccess = true,
                RedirectUris = {"https://localhost:5002/signin-oidc"},
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                }
            }
        };
        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>
        {
            //used to define the API that the identity server is protecting 
            new ApiResource
            {
                Name = "eduManagementLabApi",
                DisplayName = "EduManagementLab Api",
                Description = "Allow the application to access EduManagementLab Api on your behalf",
                Scopes = new List<string> { "eduManagementLabApi.read", "eduManagementLabApi.write"},
                ApiSecrets = new List<Secret> {new Secret("TestEduApi".Sha256())},
                UserClaims = new List<string> {"role"}
            }
        };
        public static List<TestUser> Users => new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "56892347",
                Username = "TestUser",
                Password = "Hej123!",
                Claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Email, "support@outlook.com"),
                    new Claim(JwtClaimTypes.Role, "admin"),
                    new Claim(JwtClaimTypes.WebSite, "https://EduManagementLab.com")
                }
            }
        };
    }
}
