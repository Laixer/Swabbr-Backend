using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Testing.Repositories
{
    /// <summary>
    ///     Test repository for <see cref="User"/> entities.
    /// </summary>
    public class TestUserRepository : TestRepositoryBase<User, Guid>, IUserRepository
    {
        public Task<bool> ExistsAsync(Guid id) => throw new NotImplementedException();
        public Task<bool> ExistsNicknameAsync(string nickname) => throw new NotImplementedException();
        public IAsyncEnumerable<User> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<User> GetAllVloggableUsersAsync(Navigation navigation) => throw new NotImplementedException();
        public Task<User> GetAsync(Guid id) => throw new NotImplementedException();
        public IAsyncEnumerable<User> GetFollowersAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<UserPushNotificationDetails> GetFollowersPushDetailsAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<User> GetFollowingAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public Task<UserPushNotificationDetails> GetPushDetailsAsync(Guid userId) => throw new NotImplementedException();
        public Task<UserWithStats> GetWithStatisticsAsync(Guid userId) => throw new NotImplementedException();
        public IAsyncEnumerable<UserWithRelationWrapper> SearchAsync(string query, Navigation navigation) => throw new NotImplementedException();
        public Task UpdateAsync(User entity) => throw new NotImplementedException();
    }
}
