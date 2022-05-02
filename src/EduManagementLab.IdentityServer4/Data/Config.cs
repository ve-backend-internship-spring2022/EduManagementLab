using EduManagementLab.Core.Validation;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.IdentityServer
{
    public static class Config
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
                AllowedScopes = LtiScopes,
                AllowedCorsOrigins = new List<string>
                {
                    "https://localhost:5002",
                    "https://localhost:5001",
                    "https://localhost:7134",
                    "http://localhost:5134"
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
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc"},
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
        public static ICollection<string> LtiScopes => new[]
{
            OidcConstants.StandardScopes.OpenId,
            Constants.LtiScopes.Ags.LineItem,
            Constants.LtiScopes.Ags.LineItemReadonly,
            Constants.LtiScopes.Ags.ResultReadonly,
            Constants.LtiScopes.Ags.Score,
            Constants.LtiScopes.Ags.ScoreReadonly,
            Constants.LtiScopes.Nrps.MembershipReadonly,
            "eduManagementLabApi.read",
            "eduManagementLabApi.write"
        };

        public static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }

                    context.SaveChanges();
                }

                // Define the identity resources that can be requested.
                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }

                // Define the API's that will be protected.
                if (!context.ApiResources.Any())
                {
                    foreach (var resource in ApiResources)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var scope in ApiScopes)
                    {
                        context.ApiScopes.Add(scope.ToEntity());
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}
