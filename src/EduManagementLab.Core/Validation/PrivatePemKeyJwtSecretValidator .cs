using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using EduManagementLab.Core.Configuration;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace EduManagementLab.Core.Validation
{
    public class PrivatePemKeyJwtSecretValidator : ISecretValidator
    {
        private readonly ILogger<PrivatePemKeyJwtSecretValidator> _logger;
        private readonly string _audienceUri;
        public PrivatePemKeyJwtSecretValidator(IHttpContextAccessor contextAccessor, ILogger<PrivatePemKeyJwtSecretValidator> logger)
        {
            _audienceUri = contextAccessor.HttpContext.GetIdentityServerIssuerUri();
            _logger = logger;
        }
        public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var fail = Task.FromResult(new SecretValidationResult { Success = false });
            var success = Task.FromResult(new SecretValidationResult { Success = true });

            if (parsedSecret.Type != IdentityServerConstants.ParsedSecretTypes.JwtBearer)
            {
                return fail;
            }

            if (!(parsedSecret.Credential is string token))
            {
                _logger.LogError("ParsedSecret.Credential is not a string.");
                return fail;
            }

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                _logger.LogError("ParsedSecret.Credential is not a well formed JWT.");
                return fail;
            }

            // Collect the potential public keys from the client secrets
            var secretArray = secrets as Secret[] ?? secrets.ToArray();
            var pemKeys = GetPemKeys(secretArray);

            if (!pemKeys.Any())
            {
                _logger.LogError("There are no keys available to validate the client assertion.");
                return fail;
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The token must be signed to prove the client credentials.
                RequireSignedTokens = true,
                RequireExpirationTime = true,

                IssuerSigningKeys = pemKeys,
                ValidateIssuerSigningKey = true,

                // IMS recommendation is to send any unique name as Issuer. The IMS reference 
                // implementation sends the tool name. The tool's own name for this client
                // is not known by the platform and cannot be validated.
                ValidateIssuer = false,

                // IMS recommendation is to send the base url of the authentication server
                // or the token URL.
                ValidAudiences = new[]
                {
                    _audienceUri,
                    string.Concat(_audienceUri.EnsureTrailingSlash(), "connect/token")
                },
                ValidateAudience = true
            };

            try
            {
                handler.ValidateToken(token, tokenValidationParameters, out _);

                return success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "JWT token validation error");
                return fail;
            }
        }
        private static List<RsaSecurityKey> GetPemKeys(IEnumerable<Secret> secrets)
        {
            var pemKeys = secrets
                .Where(s => s.Type.Contains(Constants.SecretTypes.PublicPemKey))
                .Select(s => s.Value)
                .ToList();

            var rsaSecurityKeys = new List<RsaSecurityKey>();

            foreach (var item in rsaSecurityKeys)
            {

            }

            foreach (var pemKey in pemKeys)
            {
                using (var keyTextReader = new StringReader(pemKey))
                {
                    // PemReader can read any PEM file. Only interested in RsaKeyParameters.
                    if (new PemReader(keyTextReader).ReadObject() is RsaKeyParameters bouncyKeyParameters)
                    {
                        var rsaParameters = new RSAParameters
                        {
                            Modulus = bouncyKeyParameters.Modulus.ToByteArrayUnsigned(),
                            Exponent = bouncyKeyParameters.Exponent.ToByteArrayUnsigned(),
                        };

                        var rsaSecurityKey = new RsaSecurityKey(rsaParameters);
                        rsaSecurityKey.KeyId = "KEYID";
                        rsaSecurityKeys.Add(rsaSecurityKey);
                    }
                }
            }

            return rsaSecurityKeys;
        }
    }
}
