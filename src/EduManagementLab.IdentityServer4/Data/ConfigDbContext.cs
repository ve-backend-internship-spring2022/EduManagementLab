using EduManagementLab.IdentityServer4.Interfaces;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.IdentityServer4.Data
{
    public class ConfigDbContext : DbContext, IConfigurationDbContext
    {
        private readonly ConfigurationStoreOptions storeOptions;

        public ConfigDbContext(DbContextOptions<ConfigDbContext> options, ConfigurationStoreOptions storeOptions)
            : base(options)
        {
            this.storeOptions = storeOptions ?? throw new ArgumentNullException(nameof(storeOptions));
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<IdentityResource> IdentityResources { get; set; }
        public DbSet<ApiResource> ApiResources { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureClientContext(storeOptions);
            modelBuilder.ConfigureResourcesContext(storeOptions);

            base.OnModelCreating(modelBuilder);
        }
    }
}
