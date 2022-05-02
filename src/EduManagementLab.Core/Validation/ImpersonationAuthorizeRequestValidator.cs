using System.Security.Claims;
using EduManagementLab.Core.Configuration;
using IdentityModel;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.Core.Validation
{
    /// <inheritdoc />
    /// <summary>
    /// Replace the subject in the authorize request, with a <see cref="T:System.Security.Claims.ClaimsPrincipal" />
    /// for the person being impersonated. For example a student in a course.
    /// </summary>
    public class ImpersonationAuthorizeRequestValidator : ICustomAuthorizeRequestValidator
    {
        private readonly ILogger<ImpersonationAuthorizeRequestValidator> _logger;

        public const string AuthenticationType = @"Impersonation";
        private readonly IConfigurationDbContext _config;

        public ImpersonationAuthorizeRequestValidator(ILogger<ImpersonationAuthorizeRequestValidator> logger, IConfigurationDbContext configurationDbContext)
        {
            _logger = logger;
            _config = configurationDbContext;
        }

        public Task ValidateAsync(CustomAuthorizeRequestValidationContext context)
        {
            var subject = context.Result.ValidatedRequest.Subject.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
            var loginHint = context.Result.ValidatedRequest.LoginHint;

            if (loginHint.IsPresent() && subject != loginHint)
            {
                _logger.LogInformation($"Impersonating subject {loginHint}.");

                // Replace the subject with the person being impersonated in login_hint
                context.Result.ValidatedRequest.Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim("sub", loginHint),
                    new Claim("auth_time", DateTime.UtcNow.ToEpochTime().ToString()),
                    new Claim("idp", "local")
                }, AuthenticationType));
            }

            return Task.CompletedTask;
        }
    }
}
