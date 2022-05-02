using EduManagementLab.IdentityServer4.Interfaces;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.IdentityServer4.Data
{
    public class PersistedDbContext : DbContext, IPersistedGrantDbContext
    {
        private readonly OperationalStoreOptions storeOptions;

        public PersistedDbContext(DbContextOptions<PersistedDbContext> options, OperationalStoreOptions storeOptions)
            : base(options)
        {
            if (storeOptions == null) throw new ArgumentNullException(nameof(storeOptions));
            this.storeOptions = storeOptions;
        }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigurePersistedGrantContext(storeOptions);

            base.OnModelCreating(modelBuilder);
        }
    }
}
