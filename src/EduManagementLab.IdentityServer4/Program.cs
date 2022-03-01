using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityServer4;
using EduManagementLab.IdentityServer4.Data;
using Microsoft.IdentityModel.Tokens;
using IdentityServer4.EntityFramework.DbContexts;
using EduManagementLab.IdentityServer4;
using EduManagementLab.IdentityServer;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AspNetIdentityServerDbcontext>(options =>
    options.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assembly)));

builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AspNetIdentityServerDbcontext>();

builder.Services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assembly));
    })
    .AddDeveloperSigningCredential();

builder.Services.AddAuthentication()
    .AddGoogle("Google", options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"];
        options.ClientSecret = builder.Configuration["Google:ClientSecret"];
    })
     .AddOpenIdConnect("oidc", options =>
     {
         options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
         options.SignOutScheme = IdentityServerConstants.SignoutScheme;
         options.SaveTokens = true;

         options.Authority = builder.Configuration["OpenIdConnect:Authority"];
         options.ClientId = builder.Configuration["OpenIdConnect:ClientId"];
         options.ClientSecret = builder.Configuration["OpenIdConnect:ClientSecret"];
         options.ResponseType = builder.Configuration["OpenIdConnect:ResponseType"];

         options.TokenValidationParameters = new TokenValidationParameters
         {
             NameClaimType = builder.Configuration["OpenIdConnect:NameClaimType"],
             RoleClaimType = builder.Configuration["OpenIdConnect:RoleClaimType"]
         };
     });

//using (var scope = builder.Services.BuildServiceProvider().CreateScope())
//{
//    var serviceProvider = scope.ServiceProvider;
//    var configcontext = serviceProvider.GetRequiredService<ConfigurationDbContext>();
//    var usermanager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
//    var aspNetdbContext = serviceProvider.GetRequiredService<AspNetIdentityServerDbcontext>();

//    DevTestData.EnsureSeedData(aspNetdbContext, configcontext, usermanager);
//}

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();