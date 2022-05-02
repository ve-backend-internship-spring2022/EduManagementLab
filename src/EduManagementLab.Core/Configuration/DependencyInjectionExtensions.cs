using EduManagementLab.Core.Validation;
using IdentityServer4.Validation;
using LtiAdvantage.Core.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace EduManagementLab.Core.Configuration
{
    public static class DependencyInjectionExtensions
    {
       /// <summary>
        /// Adds impersonation support to authorization.
        /// </summary>
        /// <remarks>
        /// Allows the application user to impersonate another user type.
        /// </remarks>
        public static IIdentityServerBuilder AddImpersonationSupport(this IIdentityServerBuilder builder)
        {
            builder.Services.AddLogging();

            builder.AddCustomAuthorizeRequestValidator<ImpersonationAuthorizeRequestValidator>();
            builder.AddSecretValidator<PrivatePemKeyJwtSecretValidator>();

            return builder;
        }

        /// <summary>
        /// Adds support for client authentication using JWT bearer assertions signed
        /// with client private key stored in PEM format rather than X509Certificate2 format.
        /// </summary>
        /// <remarks>
        /// See <see cref="IdentityServerBuilderExtensionsAdditional.AddJwtBearerClientAuthentication"/>
        /// for X509Certificate2 version.
        /// </remarks>
        public static IIdentityServerBuilder AddLtiJwtBearerClientAuthentication(this IIdentityServerBuilder builder)
        {
            builder.Services.AddLogging();

            builder.AddSecretParser<JwtBearerClientAssertionSecretParser>();
            builder.AddSecretValidator<PrivatePemKeyJwtSecretValidator>();

            return builder;
        }
    }
}
