using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swabbr.Api.Options;
using Swabbr.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Swabbr.Api.Authentication
{
    public interface ITokenService
    {
        string GenerateToken(IdentityUserTableEntity user);
    }

    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;

        public TokenService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateToken(IdentityUserTableEntity user)
        {
            ////var claims = new List<Claim>
            ////{
            ////    new Claim(ClaimTypes.PrimarySid, user.UserId.ToString()),
            ////    new Claim(ClaimTypes.Email, user.Email),
            ////};
            ////
            ////JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ////var key = Encoding.ASCII.GetBytes(_jwtOptions.Key);
            ////var tokenDescriptor = new SecurityTokenDescriptor
            ////{
            ////    Subject = new ClaimsIdentity(claims),
            ////    Expires = DateTime.UtcNow.AddDays(_jwtOptions.ExpireDays),
            ////    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            ////};
            ////var token = tokenHandler.CreateToken(tokenDescriptor);
            ////
            ////// Return JWT
            ////return tokenHandler.WriteToken(token);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.PrimarySid, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtOptions.ExpireDays));

            var token = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
