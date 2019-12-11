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

        Task<string> Authenticate(string username, string password);

        Task<User> GetByIdAsync(Guid userId);
        
        Task<IEnumerable<User>> SearchAsync(string query);
    }
}
