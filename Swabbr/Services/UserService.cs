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

        public async Task<string> Authenticate(string username, string password)
        {
            // TODO Authenticate user with un pw hash
            bool authenticated = (password.Equals(":D"));

            if (authenticated)
            {
                // Authentication succesful
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtOptions.Key);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Email, username)
                    }),
                    Expires = DateTime.UtcNow.AddDays(_jwtOptions.ExpireDays),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Return JWT string
                return tokenHandler.WriteToken(token);
            }
            else
            {
                // Authentication failed
                return null;
            }
        }

        public Task<User> GetByIdAsync(Guid userId)
        {
            return _repository.GetByIdAsync(userId);
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
