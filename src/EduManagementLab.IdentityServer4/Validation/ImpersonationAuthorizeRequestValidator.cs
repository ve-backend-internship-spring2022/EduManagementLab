using System.Security.Claims;
using EduManagementLab.Core.Services;
using EduManagementLab.IdentityServer4.Configuration;
using IdentityModel;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IdentityServer4.Validation
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
        private readonly OAuthClientService _OAuthClientService;

        public ImpersonationAuthorizeRequestValidator(ILogger<ImpersonationAuthorizeRequestValidator> logger, OAuthClientService OAuthClientService)
        {
            _logger = logger;
            _OAuthClientService = OAuthClientService;
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
