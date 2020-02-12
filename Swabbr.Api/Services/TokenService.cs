using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swabbr.Api.Authentication;
using Swabbr.Api.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Swabbr.Api.Services
{

    // TODO THOMAS Separate file
    public interface ITokenService
    {
        string GenerateToken(SwabbrIdentityUser user);
    }

    public class TokenService : ITokenService
    {
        private readonly JwtConfiguration _jwtConfig;

        public TokenService(IOptions<JwtConfiguration> jwtConfigOptions)
        {
            _jwtConfig = jwtConfigOptions.Value;
        }

        // TODO THOMAS I don't know if this is correct (could be), worth checking --> yorick heeft gecheckt, technisch klopt het (moet eigenlijk met identity server)
        public string GenerateToken(SwabbrIdentityUser user)
        {
            // Add claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtConfig.ExpireDays));

            var token = new JwtSecurityToken(
                _jwtConfig.Issuer,
                _jwtConfig.Issuer,
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}