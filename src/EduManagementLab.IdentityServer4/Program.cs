using EduManagementLab.Core.Entities;
using EduManagementLab.EfRepository;
using EduManagementLab.IdentityServer;
using EduManagementLab.IdentityServer4;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityServer4;
using EduManagementLab.IdentityServer4.Data;

var builder = WebApplication.CreateBuilder(args);

//var assembly = typeof(Program).Assembly.GetName().Name;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AspNetIdentityServerDbcontext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddIdentity<User, IdentityRole>()
//    .AddEntityFrameworkStores<DataContext>();

SeedData.EnsureSeedData(connectionString);

builder.Services.AddAuthentication()
    .AddGoogle("Google", options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

        options.ClientId = "<insert here>";
        options.ClientSecret = "<insert here>";
    });

builder.Services.AddIdentityServer()
    //.AddAspNetIdentity<User>()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString);
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString);
    })
    .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();



var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.Run();