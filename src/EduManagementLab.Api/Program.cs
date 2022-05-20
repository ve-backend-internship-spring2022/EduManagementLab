using EduManagementLab.Core.Interfaces;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.EntityFrameworkCore;
using static EduManagementLab.Api.Controllers.UsersController;
using static EduManagementLab.Api.Controllers.CoursesController;
using Microsoft.OpenApi.Models;
using LtiAdvantage;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<CourseService>();
builder.Services.AddTransient<CourseTaskService>();
builder.Services.AddAutoMapper(typeof(UserAutoMapperProfile).Assembly);
builder.Services.AddAutoMapper(typeof(CourseAutoMapperProfile).Assembly);

builder.Services.AddAuthentication("Bearer")
    .AddIdentityServerAuthentication("Bearer", options =>
    {
        options.ApiName = configuration["InteractiveServiceSettings:ApiName"];
        options.Authority = configuration["InteractiveServiceSettings:Authority"];
    });

builder.Services.AddControllers();
builder.Services.AddLtiAdvantagePolicies();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id= "oauth2"
                }
            },
            new string[]{}
        }

    });
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        In = ParameterLocation.Header,
        Flows = new OpenApiOAuthFlows()
        {
            ClientCredentials = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri(configuration["ClientCredentials:AuthorizationUrl"]),
                TokenUrl = new Uri(configuration["ClientCredentials:TokenUrl"]),
                Scopes = new Dictionary<string, string>
                {
                    {"eduManagementLabApi.read","Read Access API" },
                    {"eduManagementLabApi.write","Write Access API" },
                    {Constants.LtiScopes.Ags.Score,"Score Access" },
                    {Constants.LtiScopes.Ags.ScoreReadonly,"ReadOnly Score" },
                    {Constants.LtiScopes.Ags.LineItemReadonly, "ReadOnly lineitem" },
                    {Constants.LtiScopes.Ags.LineItem,"LineItem Access" },
                    {Constants.LtiScopes.Ags.ResultReadonly,"ReadOnly Result" },
                    {Constants.LtiScopes.Nrps.MembershipReadonly,"ReadOnly Membership" }
                }
            }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduManagementLab.IdentityServer4 v1");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapControllers().RequireAuthorization());

app.Run();
