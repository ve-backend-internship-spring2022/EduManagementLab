using EduManagementLab.Core.Entities.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Interfaces.Repositories
{
    public interface IOAuthClientRepository : IGenericRepository<OAuthClient>
    {
        IEnumerable<OAuthClient> GetOAuthClients();
        OAuthClient GetOAuthClientById(string oauthClientId, Secret secret);
    }
}
