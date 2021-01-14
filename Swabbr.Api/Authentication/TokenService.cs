using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Swabbr.Api.Authentication
{
    /// <summary>
    ///     Generates user access tokens.
    /// </summary>
    public class TokenService
    {
        private readonly JwtConfiguration _options;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public TokenService(IOptions<JwtConfiguration> options)
            => _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        /// <summary>
        ///     Generates a jwt token for a user login.
        /// </summary>
        /// <param name="user">The user that logs in.</param>
        /// <returns>The token in a wrapper.</returns>
        internal virtual TokenWrapper GenerateToken(SwabbrIdentityUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Add claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SignatureKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_options.TokenValidity));

            var token = new JwtSecurityToken(
                _options.Issuer,
                _options.Issuer,
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new TokenWrapper
            {
                DateCreated = DateTimeOffset.Now,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenExpirationTimespan = TimeSpan.FromMinutes(_options.TokenValidity)
            };
        }
    }
}
