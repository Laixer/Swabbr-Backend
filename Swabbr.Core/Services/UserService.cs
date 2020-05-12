using Laixer.Utility.Extensions;
using Microsoft.Extensions.Options;
using Swabbr.Core.Configuration;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
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

        public Task<bool> ExistsAsync(Guid userId)
        {
            return _userRepository.UserExistsAsync(userId);
        }

        public Task<bool> ExistsNicknameAsync(string nickname)
        {
            return _userRepository.NicknameExistsAsync(nickname);
        }

        /// <summary>
        /// Gets all vloggable users.
        /// </summary>
        /// <returns><see cref="SwabbrUserMinified"/> collection</returns>
        public Task<IEnumerable<SwabbrUserMinified>> GetAllVloggableUserMinifiedAsync()
        {
            return _userRepository.GetAllVloggableUserMinifiedAsync();
        }

        public Task<SwabbrUser> GetAsync(Guid userId)
        {
            return _userRepository.GetAsync(userId);
        }

        public Task<SwabbrUser> GetByEmailAsync(string email)
        {
            return _userRepository.GetByEmailAsync(email);
        }

        /// <summary>
        /// Get all followers for a <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="SwabbrUser"/> collection</returns>
        public Task<IEnumerable<SwabbrUser>> GetFollowersAsync(Guid userId)
        {
            return _userRepository.GetFollowersAsync(userId);
        }

        /// <summary>
        /// Get all users that <paramref name="userId"/> is following.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="SwabbrUser"/> collection</returns>
        public Task<IEnumerable<SwabbrUser>> GetFollowingAsync(Guid userId)
        {
            return _userRepository.GetFollowingAsync(userId);
        }

        /// <summary>
        /// Gets the notification details for a user.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="UserPushNotificationDetails"/></returns>
        public Task<UserPushNotificationDetails> GetUserPushDetailsAsync(Guid userId)
        {
            return _userRepository.GetPushDetailsAsync(userId);
        }

        /// <summary>
        /// Gets the <see cref="UserSettings"/> for a <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="UserSettings"/></returns>
        public Task<UserSettings> GetUserSettingsAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();
            return _userRepository.GetUserSettingsAsync(userId);
        }

        /// <summary>
        /// Gets the <see cref="UserStatistics"/> for a user.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="UserStatistics"/></returns>
        public Task<UserStatistics> GetUserStatisticsAsync(Guid userId)
        {
            return _userRepository.GetUserStatisticsAsync(userId);
        }

        /// <summary>
        /// Updates a <see cref="SwabbrUser"/>.
        /// </summary>
        /// <remarks>
        /// The <paramref name="wrapper"/> contains the nullable variant of the
        /// user properties. All null-values will not be stored in the data store.
        /// </remarks>
        /// <param name="wrapper"><see cref="UserUpdateWrapper"/></param>
        /// <returns>Updated <see cref="SwabbrUser"/></returns>
        public async Task<SwabbrUser> UpdateAsync(UserUpdateWrapper wrapper)
        {
            if (wrapper == null) { throw new ArgumentNullException(nameof(wrapper)); }
            wrapper.UserId.ThrowIfNullOrEmpty();
            if (!wrapper.ProfileImageBase64Encoded.IsNullOrEmpty() && !ProfileImageBase64Checker.IsValid(wrapper.ProfileImageBase64Encoded)) { throw new InvalidProfileImageStringException(); }
            if (!wrapper.Nickname.IsNullOrEmpty() && await ExistsNicknameAsync(wrapper.Nickname).ConfigureAwait(false)) { throw new NicknameExistsException(); }

            // Copy properties
            var user = await GetAsync(wrapper.UserId).ConfigureAwait(false);
            user.BirthDate = wrapper.BirthDate;
            user.Country = wrapper.Country;
            user.FirstName = wrapper.FirstName;
            user.Gender = wrapper.Gender;
            user.IsPrivate = (wrapper.IsPrivate != null) ? (bool)wrapper.IsPrivate : user.IsPrivate;
            user.LastName = wrapper.LastName;
            user.Nickname = wrapper.Nickname;
            user.ProfileImageBase64Encoded = wrapper.ProfileImageBase64Encoded;

            return await _userRepository.UpdateAsync(user).ConfigureAwait(false);
        }

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
        /// Updates the <see cref="UserSettings"/> for a <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task UpdateSettingsAsync(UserSettings userSettings)
        {
            if (userSettings == null) { throw new ArgumentNullException(nameof(userSettings)); }
            userSettings.UserId.ThrowIfNullOrEmpty();

            if (userSettings.DailyVlogRequestLimit < 0) { throw new ArgumentOutOfRangeException(nameof(userSettings)); }
            if (userSettings.DailyVlogRequestLimit > config.DailyVlogRequestLimit) { throw new InvalidOperationException($"Daily vlog request limit can't be greater than {config.DailyVlogRequestLimit}"); }

            return _userRepository.UpdateUserSettingsAsync(userSettings);
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
