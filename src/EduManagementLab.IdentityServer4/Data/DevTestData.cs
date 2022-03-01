using EduManagementLab.Core.Entities;
using EduManagementLab.EfRepository;
using EduManagementLab.IdentityServer;
using EduManagementLab.IdentityServer4.Data;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduManagementLab.IdentityServer4
{
    public class DevTestData
    {
        public static void EnsureSeedData(AspNetIdentityServerDbcontext dbContext, ConfigurationDbContext configcontext, UserManager<IdentityUser> usermanager)
        {           
            EnsureUsers(dbContext, usermanager);
            EnsureSeedData(configcontext);
        }

        private static void EnsureUsers(AspNetIdentityServerDbcontext dbContext, UserManager<IdentityUser> usermanager)
        {
            var angella = new IdentityUser
            {
                UserName = "angellatest",
                Email = "angella.freeman@email.com",
                EmailConfirmed = true,
            };

            if (!dbContext.Users.Any(u => u.Email == angella.Email))
            {
                var result = usermanager.CreateAsync(angella, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                result = usermanager.AddClaimsAsync(
                    angella,
                    new Claim[]
                    {
                            new Claim(JwtClaimTypes.Name, "Angella Freeman"),
                            new Claim(JwtClaimTypes.GivenName, "Angella"),
                            new Claim(JwtClaimTypes.FamilyName, "Freeman"),
                            new Claim(JwtClaimTypes.WebSite, "http://angellafreeman.com"),
                            new Claim("location", "somewhere")
                    }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
            dbContext.SaveChanges();
        }
        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in TestData.Clients.ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in TestData.IdentityResources.ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in TestData.ApiScopes.ToList())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in TestData.ApiResources.ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }
        }
    }
}
