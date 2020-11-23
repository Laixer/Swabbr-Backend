using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Repository for <see cref="SwabbrUser"/> entities.
    /// </summary>
    public interface IUserRepository : IRepository<SwabbrUser, Guid>, ICudFunctionality<SwabbrUser, Guid>
    {
        Task<SwabbrUser> GetByEmailAsync(string email);

        /// <summary>
        ///     Gets a user with its statistics.
        /// </summary>
        /// <param name="userId">The internal user id.</param>
        /// <returns>The user entity with statistics.</returns>
        Task<SwabbrUserWithStats> GetWithStatisticsAsync(Guid userId);

        Task<bool> UserExistsAsync(Guid userId);

        Task<bool> NicknameExistsAsync(string nickname);

        // TODO Move to withstats
        Task<IEnumerable<SwabbrUser>> GetFollowingAsync(Guid userId);

        // TODO Move to withstats
        Task<IEnumerable<SwabbrUser>> GetFollowersAsync(Guid userId);

        Task<IEnumerable<UserPushNotificationDetails>> GetFollowersPushDetailsAsync(Guid userId);

        Task<UserPushNotificationDetails> GetPushDetailsAsync(Guid userId);

        Task<IEnumerable<SwabbrUser>> GetVlogRequestableUsersAsync(DateTimeOffset from, TimeSpan timeSpan);

        /// <summary>
        ///     Gets a collection of all users that are eligible
        ///     for a vlog request.
        /// </summary>
        IAsyncEnumerable<SwabbrUser> GetAllVloggableUsersAsync();

        Task<SwabbrUser> GetUserFromVlogAsync(Guid vlogId);

        Task UpdateLocationAsync(Guid userId, double longitude, double latitude);

        Task UpdateTimeZoneAsync(Guid userId, TimeZoneInfo newTimeZone);

    }

}
