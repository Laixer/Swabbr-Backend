using Microsoft.Extensions.Options;
using Swabbr.Core.Configuration;
using Swabbr.Core.Entities;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    /// <see cref="SwabbrUser"/> service.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly SwabbrConfiguration config;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserService(IUserRepository userRepository,
            IOptions<SwabbrConfiguration> options)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            options.Value.ThrowIfInvalid();
            config = options.Value;
        }

        public Task<bool> ExistsAsync(Guid userId) => _userRepository.UserExistsAsync(userId);

        public Task<bool> ExistsNicknameAsync(string nickname) => _userRepository.NicknameExistsAsync(nickname);

        /// <summary>
        ///     Gets all users which are eligible for a vlog request.
        /// </summary>
        /// <returns>Vloggable user collection</returns>
        public Task<IEnumerable<SwabbrUser>> GetAllVloggableUsersAsync() 
            => _userRepository.GetAllVloggableUsersAsync();

        public Task<SwabbrUser> GetAsync(Guid userId) => _userRepository.GetAsync(userId);

        public Task<SwabbrUser> GetByEmailAsync(string email) => _userRepository.GetByEmailAsync(email);

        /// <summary>
        /// Get all followers for a <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="SwabbrUser"/> collection</returns>
        public Task<IEnumerable<SwabbrUser>> GetFollowersAsync(Guid userId) => _userRepository.GetFollowersAsync(userId);

        /// <summary>
        /// Get all users that <paramref name="userId"/> is following.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="SwabbrUser"/> collection</returns>
        public Task<IEnumerable<SwabbrUser>> GetFollowingAsync(Guid userId) => _userRepository.GetFollowingAsync(userId);

        /// <summary>
        /// Gets the notification details for a user.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="UserPushNotificationDetails"/></returns>
        public Task<UserPushNotificationDetails> GetUserPushDetailsAsync(Guid userId) => _userRepository.GetPushDetailsAsync(userId);

        /// <summary>
        ///     Gets a user with corresponding statistics.
        /// </summary>
        /// <param name="userId">The internal user id.</param>
        /// <returns>User entity with statistics.</returns>
        public Task<SwabbrUserWithStats> GetWithStatisticsAsync(Guid userId)
            => _userRepository.GetWithStatisticsAsync(userId);

        // TODO Should this do any of the work that UserSettingsController.UpdateAsync does?
        /// <summary>
        ///     This updates a user in our database.
        /// </summary>
        /// <param name="user">The updated user entity.</param>
        /// <returns>The user entity after the update operation.</returns>
        public Task<SwabbrUser> UpdateAsync(SwabbrUser user)
            => _userRepository.UpdateAsync(user);

        /// <summary>
        /// Update the user location.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public Task UpdateLocationAsync(Guid userId, double longitude, double latitude)
        {
            userId.ThrowIfNullOrEmpty();
            return _userRepository.UpdateLocationAsync(userId, longitude, latitude);
        }

        /// <summary>
        /// Update the user timezone.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newTimeZone"></param>
        /// <returns></returns>
        public Task UpdateTimeZoneAsync(Guid userId, TimeZoneInfo newTimeZone)
        {
            userId.ThrowIfNullOrEmpty();
            if (newTimeZone == null) { throw new ArgumentNullException(nameof(newTimeZone)); }
            return _userRepository.UpdateTimeZoneAsync(userId, newTimeZone);
        }
    }
}
