
using EduManagementLab.Core.Entities.client;
using EduManagementLab.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.EfRepository.Repositories
{
    internal class OAuthClientRepository : GenericRepository<OAuthClient>, IOAuthClientRepository
    {
        public readonly DataContext _context;
        public OAuthClientRepository(DataContext context) : base(context)
        {
            _context = context;
        }
        public IEnumerable<OAuthClient> GetOAuthClients()
        {
            return _context.OAuthClients
                .Include(s => s.ClientSecrets)
                .Include(s => s.RedirectUris)
                .Include(s => s.AllowedGrantTypes)
                .Include(s => s.AllowedScopes)
                .Include(s => s.AllowedCorsOrigins)
                .Include(s => s.PostLogoutRedirectUris)
                .ToList();
        }
        public OAuthClient FindClientByClientId(string oauthClientId)
        {
            return _context.OAuthClients
                .Include(s => s.ClientSecrets)
                .Include(s => s.RedirectUris)
                .Include(s => s.AllowedGrantTypes)
                .Include(s => s.AllowedScopes)
                .Include(s => s.AllowedCorsOrigins)
                .Include(s => s.PostLogoutRedirectUris)
                .FirstOrDefault(c => c.ClientId == oauthClientId);
        }
    }
}