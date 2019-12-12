using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swabbr.Api.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Api.Services
{
    public class UserService : IUserService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository, IOptions<JwtOptions> jwtOptions)
        {
            _repository = repository;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<string> GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.PrimarySid, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                }),
                Expires = DateTime.UtcNow.AddDays(_jwtOptions.ExpireDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return JWT
            return tokenHandler.WriteToken(token);
        }

        public Task<User> GetByIdAsync(Guid userId)
        {
            return _repository.GetByIdAsync(userId);
        }

        public Task<User> GetByEmailAsync(string email)
        {
            return _repository.GetByEmailAsync(email);
        }

        public Task<IEnumerable<User>> SearchAsync(string query)
        {
            // TODO Offset and limit are hardcoded for now
            return _repository.SearchAsync(query, 0, 100);
        }

        public Task TempDeleteTables()
        {
            return _repository.TempDeleteTables();
        }
    }
}
