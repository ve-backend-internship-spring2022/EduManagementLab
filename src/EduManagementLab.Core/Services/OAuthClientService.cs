using EduManagementLab.Core.Entities.client;
using EduManagementLab.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Services
{
    public partial class OAuthClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OAuthClientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<OAuthClient> GetOAuthClients()
        {
            return _unitOfWork.OAuthClients.GetAll();
        }
        public OAuthClient GetOAuthClientById(string clientId)
        {
            return _unitOfWork.OAuthClients.GetById(Guid.Parse(clientId));
        }
        public OAuthClient DeleteOAuthClientById(Guid Id)
        {
            var targetClient = _unitOfWork.OAuthClients.GetById(Id);
            _unitOfWork.OAuthClients.Remove(targetClient);
            return targetClient;
        }
        public OAuthClient CreateOAuthClient(string ClientId, string clientName, string launchUrl, string publicKey)
        {
            var newOAuthClient = new OAuthClient
            {
                ClientId = ClientId,
                ClientName = clientName,
                AllowedGrantTypes = new List<ClientGrantType>() { new ClientGrantType { GrantType = "ImplicitAndClientCredentials" } },
                AllowedScopes = new List<ClientScope>() {
                    new ClientScope { Scope = "eduManagementLabApi.read" },
                    new ClientScope { Scope = "eduManagementLabApi.write" },
                    new ClientScope { Scope = "https://purl.imsglobal.org/spec/lti-ags/scope/lineitem" },
                    new ClientScope { Scope = "https://purl.imsglobal.org/spec/lti-ags/scope/lineitem.readonly" },
                    new ClientScope { Scope = "https://purl.imsglobal.org/spec/lti-ags/scope/result.readonly" },
                    new ClientScope { Scope = "https://purl.imsglobal.org/spec/lti-ags/scope/score" },
                    new ClientScope { Scope = "https://purl.imsglobal.org/spec/lti-ags/scope/score.readonly" },
                    new ClientScope { Scope = "https://purl.imsglobal.org/spec/lti-nrps/scope/contextmembership.readonly" },
                    new ClientScope { Scope = "openid" },
                    },
                ClientSecrets = new List<ClientSecret>
                {
                    new ClientSecret { Type = "PublicPemKey", Value = publicKey }
                },
                AllowedCorsOrigins = new List<ClientCorsOrigin>
                {
                    new ClientCorsOrigin{Origin= "https://localhost:5001"},
                    new ClientCorsOrigin{Origin= "https://localhost:5002"},
                    new ClientCorsOrigin{Origin= "https://localhost:44308"},
                    new ClientCorsOrigin{Origin= "https://localhost:44338"},
                },
                RedirectUris = new List<ClientRedirectUri> { new ClientRedirectUri { RedirectUri = launchUrl } },
                RequireConsent = false
            };


            _unitOfWork.OAuthClients.Add(newOAuthClient);
            _unitOfWork.Complete();
            return newOAuthClient;

        }

        public bool ValidateCredentials(string clientId, string secret)
        {
            var targetClient = GetOAuthClientById(clientId);
            if (targetClient != null)
            {
                return targetClient.ClientSecrets.Equals(secret);
            }

            return false;
        }
    }
}