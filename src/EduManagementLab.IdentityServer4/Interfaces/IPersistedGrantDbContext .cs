using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.IdentityServer4.Interfaces
{
    public interface IPersistedGrantDbContext
    {
        public interface IPersistedGrantDbContext : IDisposable
        {
            DbSet<PersistedGrant> PersistedGrants { get; set; }

            int SaveChanges();
            Task<int> SaveChangesAsync();
        }
    }
}
