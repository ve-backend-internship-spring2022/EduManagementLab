using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.IdentityServer4.Data
{
    public class AspNetIdentityServerDbcontext : IdentityDbContext
    {
        public AspNetIdentityServerDbcontext(DbContextOptions<AspNetIdentityServerDbcontext> options) : base(options)
        {
        }
    }
}
