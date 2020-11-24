using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{
    internal class UserRepository : RepositoryBase, IUserRepository
    {
        public Task<Guid> CreateAsync(SwabbrUser entity) => throw new NotImplementedException();
        public Task DeleteAsync(Guid id) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(Guid id) => throw new NotImplementedException();
        public IAsyncEnumerable<SwabbrUser> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<SwabbrUser> GetAllVloggableUsersAsync(Navigation navigation) => throw new NotImplementedException();
        public Task<SwabbrUser> GetAsync(Guid id) => throw new NotImplementedException();
        public IAsyncEnumerable<SwabbrUser> GetFollowersAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<UserPushNotificationDetails> GetFollowersPushDetailsAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<SwabbrUser> GetFollowingAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public Task<UserPushNotificationDetails> GetPushDetailsAsync(Guid userId) => throw new NotImplementedException();
        public Task<SwabbrUserWithStats> GetWithStatisticsAsync(Guid userId) => throw new NotImplementedException();
        public Task<bool> NicknameExistsAsync(string nickname) => throw new NotImplementedException();
        public IAsyncEnumerable<SwabbrUserWithStats> SearchAsync(string query, Navigation navigation) => throw new NotImplementedException();
        public Task UpdateAsync(SwabbrUser entity) => throw new NotImplementedException();
        public Task UpdateLocationAsync(Guid userId, double longitude, double latitude) => throw new NotImplementedException();
        public Task UpdateTimeZoneAsync(Guid userId, TimeZoneInfo newTimeZone) => throw new NotImplementedException();
    }
}
