using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swabbr.Api.Authentication;
using Swabbr.Api.Options;
using Swabbr.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Api.Services
{
    public interface ITokenService
    {
        string GenerateToken(SwabbrIdentityUser user);
    }

    public class TokenService : ITokenService
    {
        private readonly JwtConfiguration _jwtOptions;

        public TokenService(IOptions<JwtConfiguration> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateToken(SwabbrIdentityUser user)
        {
            // Add claims
            var claims = new List<Claim>
            {
                new Claim(SwabbrClaimTypes.UserId, user.UserId.ToString()),
                new Claim(SwabbrClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtOptions.ExpireDays));

            var token = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Issuer,
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}