using Swabbr.Core.Abstractions;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Service to handle user related operations.
    /// </summary>
    public class UserService :  AppServiceBase, IUserService
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public UserService(AppContext appContext,
            IUserRepository userRepository)
        {
            AppContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        ///     Checks if a user exists in our data store.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public Task<bool> ExistsAsync(Guid userId) 
            => _userRepository.ExistsAsync(userId);

        /// <summary>
        ///     Checks if a nickname exists in our data store.
        /// </summary>
        /// <param name="nickname">The nickname to check.</param>
        public Task<bool> ExistsNicknameAsync(string nickname) 
            => _userRepository.ExistsNicknameAsync(nickname);

        /// <summary>
        ///     Gets all users which are eligible for a vlog request.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vloggable user collection</returns>
        public IAsyncEnumerable<SwabbrUser> GetAllVloggableUsersAsync(Navigation navigation) 
            => _userRepository.GetAllVloggableUsersAsync(navigation);

        /// <summary>
        ///     Get a single user from our data store.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The user.</returns>
        public Task<SwabbrUser> GetAsync(Guid userId) 
            => _userRepository.GetAsync(userId);

        /// <summary>
        ///     Gets all followers for a user.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All followers.</returns>
        public IAsyncEnumerable<SwabbrUser> GetFollowersAsync(Guid userId, Navigation navigation)
            => _userRepository.GetFollowersAsync(userId, navigation);

        /// <summary>
        ///     Gets all users a user is following.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All users followed by <paramref name="userId"/>.</returns>
        public IAsyncEnumerable<SwabbrUser> GetFollowingAsync(Guid userId, Navigation navigation)
            => _userRepository.GetFollowingAsync(userId, navigation);

        /// <summary>
        ///     Gets the details required to send a push notification.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <returns>Push notification details.</returns>
        public Task<UserPushNotificationDetails> GetUserPushDetailsAsync(Guid userId) 
            => _userRepository.GetPushDetailsAsync(userId);

        /// <summary>
        ///     Gets a user with corresponding statistics.
        /// </summary>
        /// <param name="userId">The internal user id.</param>
        /// <returns>User entity with statistics.</returns>
        public Task<SwabbrUserWithStats> GetWithStatisticsAsync(Guid userId)
            => _userRepository.GetWithStatisticsAsync(userId);

        /// <summary>
        ///     Search for users in our data store.
        /// </summary>
        /// <param name="query">Search string.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>User search result set.</returns>
        public IAsyncEnumerable<SwabbrUserWithStats> SearchAsync(string query, Navigation navigation)
            => _userRepository.SearchAsync(query, navigation);

        // TODO Should this do any of the work that UserSettingsController.UpdateAsync does?
        /// <summary>
        ///     This updates the current user in our database.
        /// </summary>
        /// <param name="user">The updated user entity.</param>
        /// <returns>The user entity after the update operation.</returns>
        public Task UpdateAsync(SwabbrUser user)
            => _userRepository.UpdateAsync(user);

        /// <summary>
        ///     Updates a user location in our data store.
        /// </summary>
        /// <remarks>
        ///     The user id is extracted from the context.
        /// </remarks>
        /// <param name="longitude">New longitude coordinate.</param>
        /// <param name="latitude">New latitude coordinate.</param>
        public async Task UpdateLocationAsync(double longitude, double latitude)
        {
            if (!AppContext.HasUser)
            {
                throw new InvalidOperationException();
            }

            var user = await _userRepository.GetAsync(AppContext.UserId);

            user.Longitude = longitude;
            user.Latitude = latitude;

            await UpdateAsync(user);
        }

        /// <summary>
        ///     Updates a user timezone in our data store.
        /// </summary>
        /// <remarks>
        ///     The user id is extracted from the context.
        /// </remarks>
        /// <param name="newTimeZone">The new user timezone.</param>
        public async Task UpdateTimeZoneAsync(TimeZoneInfo newTimeZone)
        {
            if (!AppContext.HasUser)
            {
                throw new InvalidOperationException();
            }

            var user = await _userRepository.GetAsync(AppContext.UserId);

            user.Timezone = newTimeZone;

            await UpdateAsync(user);
        }
    }
}
