using Microsoft.EntityFrameworkCore;
using EduManagementLab.Core.Interfaces;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using EduManagementLab.IdentityServer;
using EduManagementLab.IdentityServer4.Configuration;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using EduManagementLab.IdentityServer4.Services;
using IdentityServer4.Services;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assembly)));

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<CourseService>();
builder.Services.AddTransient<CourseTaskService>();
builder.Services.AddTransient<ToolService>();
builder.Services.AddTransient<ResourceLinkService>();
builder.Services.AddTransient<OAuthClientService>();

builder.Services.AddSingleton<ICorsPolicyService>((container) =>
{
    //Solution for API Authorization http://docs.identityserver.io/en/latest/topics/cors.html
    var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();
    return new DefaultCorsPolicyService(logger)
    {
        AllowedOrigins = { "https://localhost:7134" }
    };
});

builder.Services.AddIdentityServer(x => x.InputLengthRestrictions.Scope = 500)
    .AddDeveloperSigningCredential()
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiResources(Config.ApiResources)
    //.AddInMemoryClients(Config.Clients)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddImpersonationSupport()
    .AddProfileService<CustomProfileService>()
    .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>()
    .AddClientStore<CustomClientStore>()
    .AddLtiJwtBearerClientAuthentication();

builder.Services.AddAuthentication();
//-- End --

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();