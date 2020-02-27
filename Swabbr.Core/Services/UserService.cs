using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// User service.
    /// TODO This is never used.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public Task<SwabbrUser> CreateAsync(SwabbrUser user)
        {
            return _userRepository.CreateAsync(user);
        }

        public Task<bool> ExistsAsync(Guid userId)
        {
            return _userRepository.UserExistsAsync(userId);
        }

        public Task<SwabbrUser> GetAsync(Guid userId)
        {
            return _userRepository.GetAsync(userId);
        }

        public Task<SwabbrUser> GetByEmailAsync(string email)
        {
            return _userRepository.GetByEmailAsync(email);
        }

        public Task<IEnumerable<SwabbrUser>> SearchAsync(string query, uint offset, uint limit)
        {
            return _userRepository.SearchAsync(query, offset, limit);
        }

        public Task<SwabbrUser> UpdateAsync(SwabbrUser user)
        {
            return _userRepository.UpdateAsync(user);
        }
    }
}
