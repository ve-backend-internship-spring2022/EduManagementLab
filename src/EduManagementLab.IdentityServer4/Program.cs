using EduManagementLab.IdentityServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServer()
        .AddInMemoryClients(Config.Clients())
        .AddInMemoryIdentityResources(Config.IdentityResources())
        .AddInMemoryApiResources(Config.ApiResources())
        .AddInMemoryApiScopes(Config.ApiScopes())
        .AddTestUsers(Config.TestUsers())
        .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseIdentityServer();

app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.Run();