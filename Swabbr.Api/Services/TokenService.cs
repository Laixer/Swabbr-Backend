using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swabbr.Api.Authentication;
using Swabbr.Api.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Swabbr.Api.Services
{

    /// <summary>
    /// Generates user access tokens.
    /// </summary>
    public class TokenService : ITokenService
    {

        private readonly JwtConfiguration _jwtConfig;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public TokenService(IOptions<JwtConfiguration> config)
        {
            if (config == null || config.Value == null) { throw new ArgumentNullException(nameof(config)); }
            _jwtConfig = config.Value;
            _jwtConfig.ThrowIfInvalid();
        }

        /// <summary>
        /// Generates a token for a user.
        /// </summary>
        /// <param name="user"><see cref="SwabbrIdentityUser"/></param>
        /// <returns><see cref="TokenWrapper"/></returns>
        public TokenWrapper GenerateToken(SwabbrIdentityUser user)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }

            // Add claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtConfig.ExpireMinutes));

            var token = new JwtSecurityToken(
                _jwtConfig.Issuer,
                _jwtConfig.Issuer,
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new TokenWrapper
            {
                CreateDate = DateTimeOffset.Now,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenExpirationTimespan = TimeSpan.FromMinutes(_jwtConfig.ExpireMinutes)
            };
        }

    }

}
