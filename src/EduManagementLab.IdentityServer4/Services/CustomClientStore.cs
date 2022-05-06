using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Entities.client;
using EduManagementLab.Core.Services;
using EduManagementLab.IdentityServer;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;

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
            //var result = _oauthClientService.GetOAuthClientById(clientId);
            //var newClient = new Client
            //{

            //};
            return Task.FromResult(Config.Clients.FirstOrDefault(c => c.ClientId == clientId));
        }

        //public Task ValidateAsync(ClientConfigurationValidationContext context)
        //{
        //    if (_oauthClientService.ValidateCredentials(
        //        context.Client.ClientId, context.Client.ClientSecrets.First(c => c.Type == "PublicKey").Value))
        //    {
        //        var result = _oauthClientService.GetOAuthClientById(Guid.Parse(context.Client.ClientId));
        //    }

        //    return Task.FromResult(0);
        //}
    }
}