using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Interfaces.Services;
using Swabbr.AzureMediaServices.Utility;
using Swabbr.Core.Extensions;
using Swabbr.Core.Utility;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Swabbr.AzureMediaServices.Services
{
    /// <summary>
    /// Generates tokens for resource playback in AMS.
    /// </summary>
    public sealed class AMSTokenService : IAMSTokenService
    {
        private readonly AMSConfiguration config;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSTokenService(IOptions<AMSConfiguration> options)
        {
            if (options == null || options.Value == null) { throw new ArgumentNullException(nameof(options)); }
            config = options.Value;
            config.ThrowIfInvalid();
        }

        /// <summary>
        /// Generates a new token for a streaming locator.
        /// </summary>
        /// <param name="keyIdentifier">Streaming locator key identifier</param>
        /// <returns>Token</returns>
        public string GenerateToken(string keyIdentifier)
        {
            keyIdentifier.ThrowIfNullOrEmpty();
            var tokenSigningKey = new SymmetricSecurityKey(new UTF8Encoding().GetBytes(config.TokenSecret));

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: config.TokenIssuer,
                audience: config.TokenAudience,
                claims: new Claim[] { new Claim(ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim.ClaimType, keyIdentifier) },
                notBefore: DateTime.UtcNow.AddMinutes(-AMSConstants.PlaybackTokenTimeSubstractionMinutes),
                expires: DateTime.UtcNow.AddMinutes(config.TokenValidMinutes),
                signingCredentials: new SigningCredentials(tokenSigningKey, SecurityAlgorithms.HmacSha256, SecurityAlgorithms.Sha256Digest)));
        }
    }
}
