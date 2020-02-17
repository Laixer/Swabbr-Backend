using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<SwabbrUser> CreateAsync(SwabbrUser user)
        {
            return await _userRepository.CreateAsync(user);
        }

        public async Task<bool> ExistsAsync(Guid userId)
        {
            return await _userRepository.UserExistsAsync(userId);
        }

        public async Task<SwabbrUser> GetAsync(Guid userId)
        {
            return await _userRepository.GetAsync(userId);
        }

        public async Task<SwabbrUser> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<SwabbrUser>> SearchAsync(string query, uint offset, uint limit)
        {
            return await _userRepository.SearchAsync(query, offset, limit);
        }

        public async Task<SwabbrUser> UpdateAsync(SwabbrUser user)
        {
            return await _userRepository.UpdateAsync(user);
        }
    }
}
