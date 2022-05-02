using Microsoft.EntityFrameworkCore;
using EduManagementLab.Core.Interfaces;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using EduManagementLab.IdentityServer;
using EduManagementLab.IdentityServer4.Interfaces;
using EduManagementLab.IdentityServer4.Data;
using EduManagementLab.Core.Configuration;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assembly)));

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<CourseService>();
builder.Services.AddTransient<CourseLineItemService>();
builder.Services.AddTransient<ToolService>();
builder.Services.AddTransient<ResourceLinkService>();

//-- Specification to use custom IdentityServerDbContext insted of inMemory --
var identity = builder.Configuration.GetConnectionString("IdentityServerDbContext");
builder.Services.AddDbContext<ConfigDbContext>(options =>
    options.UseSqlServer(identity));

builder.Services.AddDbContext<PersistedDbContext>(options =>
    options.UseSqlServer(identity));

builder.Services.AddTransient<IConfigurationDbContext, ConfigDbContext>();
builder.Services.AddTransient<IPersistedGrantDbContext, PersistedDbContext>();

builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = build =>
            build.UseSqlServer(builder.Configuration.GetConnectionString("IdentityServerDbContext"),
                sql => sql.MigrationsAssembly(assembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = build =>
             build.UseSqlServer(builder.Configuration.GetConnectionString("IdentityServerDbContext"),
                 sql => sql.MigrationsAssembly(assembly));
        // Clean up expired tokens
        options.EnableTokenCleanup = true;
        options.EnableTokenCleanup = false;
        options.TokenCleanupInterval = 30;
    })
    .AddImpersonationSupport()
    .AddProfileService<CustomProfileService>()
    .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>()
    .AddLtiJwtBearerClientAuthentication();

builder.Services.AddAuthentication();
//-- End --

var app = builder.Build();
Config.InitializeDatabase(app);
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