using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.IdentityServer4.Interfaces
{
    public interface IConfigurationDbContext : IDisposable
    {
        DbSet<Client> Clients { get; set; }
        DbSet<IdentityResource> IdentityResources { get; set; }
        DbSet<ApiResource> ApiResources { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
