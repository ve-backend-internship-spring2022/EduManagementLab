using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Entities.client;
using EduManagementLab.Core.Services;
using EduManagementLab.IdentityServer;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IDS4Secret = IdentityServer4.Models.Secret;

namespace EduManagementLab.IdentityServer4.Services
{
    public class CustomClientStore : IClientStore
    {
        private readonly OAuthClientService _oauthClientService;
        public CustomClientStore(OAuthClientService oauthClientService)
        {
            _oauthClientService = oauthClientService;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var result = _oauthClientService.FindClientByClientId(clientId);
            var newClient = new Client
            {
                ClientId = clientId,
                ClientName = result.ClientName,
                ClientSecrets = result.ClientSecrets.Select(c => new IDS4Secret(c.Value)).ToList(),
                AllowedGrantTypes = result.AllowedGrantTypes.Select(c => c.GrantType).ToList(),
                AllowedScopes = result.AllowedScopes.Select(c => c.Scope).ToList(),
                AllowedCorsOrigins = result.AllowedCorsOrigins.Select(c => c.Origin).ToList(),
                PostLogoutRedirectUris = result.PostLogoutRedirectUris.Select(c => c.PostLogoutRedirectUri).ToList(),
                RedirectUris = result.RedirectUris.Select(c => c.RedirectUri).ToList(),
                RequireConsent = false
            }; 

            return Task.FromResult(newClient);
        }
    }
}