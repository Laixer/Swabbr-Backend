using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Api.Services
{
    public interface IUserService
    {
        // TODO Remove, used temporarily to delete tables to reduce storage costs overnight
        Task TempDeleteTables();

        Task<string> GenerateAccessToken(User user);

        Task<User> GetByIdAsync(Guid userId);

        Task<User> GetByEmailAsync(string email);
        
        Task<IEnumerable<User>> SearchAsync(string query);
    }
}
