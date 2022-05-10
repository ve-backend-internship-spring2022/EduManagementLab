using EduManagementLab.EfRepository;
using EduManagementLab.IdentityServer4.Validation;
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
    }
}
