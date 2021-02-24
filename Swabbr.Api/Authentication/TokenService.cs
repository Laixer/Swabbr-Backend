using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Api.Authentication
{
    /// <summary>
    ///     Generates user access tokens and refresh tokens.
    /// </summary>
    public class TokenService
    {
        private readonly JwtConfiguration _options;
        private readonly IUserRefreshTokenRepository _userRefreshTokenRepository;

        private readonly SymmetricSecurityKey signingKey;
        private readonly SigningCredentials signingCredentials;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public TokenService(IOptions<JwtConfiguration> options,
            IUserRefreshTokenRepository userRefreshTokenRepository)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _userRefreshTokenRepository = userRefreshTokenRepository ?? throw new ArgumentNullException(nameof(userRefreshTokenRepository));

            signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SignatureKey));
            signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        }

        /// <summary>
        ///     Generates a <see cref="TokenWrapper"/> for a user login along with a
        ///     refresh token.
        /// </summary>
        /// <param name="userId">The id of the user that wants to log in.</param>
        /// <returns>The new token wrapper.</returns>
        internal virtual async Task<TokenWrapper> GenerateTokenAsync(Guid userId)
        {
            // Add claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                _options.Issuer,
                _options.Audience,
                claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_options.TokenValidity)),
                signingCredentials: signingCredentials
            );

            // Generate and store a new refresh token.
            var refreshToken = GenerateRefreshToken();
            await _userRefreshTokenRepository.StoreRefreshTokenHashAsync(userId, refreshToken);

            return new TokenWrapper
            {
                UserId = userId,
                DateCreated = DateTimeOffset.Now,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenExpirationInMinutes = _options.TokenValidity, // Options is in minutes.
                RefreshToken = refreshToken,
                RefreshTokenExpirationInMinutes = _options.RefreshTokenValidity, // Options is in minutes.
            };
        }

        /// <summary>
        ///     Refresh an expired authentication token and generate a new <see cref="TokenWrapper"/>
        ///     containing a new refresh token as well. The expiration for the refresh token is checked
        ///     based on the valid-from date of the token and the <see cref="_options"/> values.
        /// </summary>
        /// <param name="expiredToken">The expired jwt token.</param>
        /// <param name="refreshToken">Valid refresh token.</param>
        /// <returns>New token wrapper with new refresh token.</returns>
        internal virtual async Task<TokenWrapper> RefreshTokenAsync(string expiredToken, string refreshToken)
        {
            // First extract the user id. This method will throw if the expiredToken has an invalid format.
            var principal = GetPrincipalFromExpiredToken(expiredToken);
            var identity = principal.Identity as ClaimsIdentity; // Cast required to access the claims.
            if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            {
                throw new AuthenticationException("Couldn't extract user id from token principal");
            }

            // Validate refresh token in db based on user id
            if (!await _userRefreshTokenRepository.ValidateRefreshTokenAsync(userId, refreshToken))
            {
                throw new AuthenticationException("Refresh token was invalid");
            }

            // Then validate the lifetime.
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(expiredToken);
            if (DateTimeOffset.UtcNow >= jwtToken.ValidFrom.AddMinutes(_options.RefreshTokenValidity))
            {
                throw new AuthenticationException("Refresh token has expired");
            }

            // If we reach this the user may receive a new access token.
            return await GenerateTokenAsync(userId);
        }

        /// <summary>
        ///     Invalidates a refresh token for future use.
        /// </summary>
        /// <param name="userId">The user id that the refresh token would belong to.</param>
        internal virtual Task RevokeRefreshTokenAsync(Guid userId)
            => _userRefreshTokenRepository.RevokeRefreshTokenAsync(userId);

        /// <summary>
        ///     Generates a new refresh token.
        /// </summary>
        /// <returns>Refresh token.</returns>
        private static string GenerateRefreshToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var randomNumber = new byte[32];
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        ///     Extract the <see cref="ClaimsPrincipal"/> from an expired token.
        ///     The validator used in this method doesn't validate the lifetime.
        /// </summary>
        /// <remarks>
        ///     This will also work as expected when the token is not yet expired.
        /// </remarks>
        /// <param name="expiredToken">Expired jwt token.</param>
        /// <returns>Corresponding <see cref="ClaimsPrincipal"/>.</returns>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string expiredToken)
        {
            // First validate our token and and extract the user parameters from it.
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SignatureKey)),
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                // Don't validate the lifetime as we expect the token to be expired.
                // A non-expired token will not be a problem, just continue.
                ValidateLifetime = false,
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(expiredToken, tokenValidationParameters, out SecurityToken securityToken);

                // Check if we could parse it and if the signing algorithm matches.
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new AuthenticationException("Invalid token format");
                }

                return principal;
            }
            catch (SecurityTokenException e)
            {
                throw new AuthenticationException("Token was invalid", e);
            }
        }
    }
}
