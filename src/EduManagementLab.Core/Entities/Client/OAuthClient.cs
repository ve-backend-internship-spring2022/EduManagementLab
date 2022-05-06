using EduManagementLab.Core.Entities.Client;

namespace EduManagementLab.Core.Entities.client
{
    public class OAuthClient
    {
        public Guid Id { get; set; }
        public string ClientId { get; set; }
        public string ProtocolType { get; set; } = "oidc";
        public List<ClientSecret> ClientSecrets { get; set; }
        public bool RequireClientSecret { get; set; } = true;
        public string ClientName { get; set; }
        public bool RequireConsent { get; set; } = true;
        public List<ClientGrantType> AllowedGrantTypes { get; set; }
        public List<ClientRedirectUri> RedirectUris { get; set; }
        public List<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public List<ClientScope> AllowedScopes { get; set; }
        public List<ClientIdPRestriction> IdentityProviderRestrictions { get; set; }
        public List<ClientClaim> Claims { get; set; }
        public List<ClientCorsOrigin> AllowedCorsOrigins { get; set; }
        public List<ClientProperty> Properties { get; set; }
    }
}