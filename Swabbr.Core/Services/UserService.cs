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
        private readonly IEntityStorageUriService _entityStorageUriService;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public UserService(AppContext appContext,
            IUserRepository userRepository,
            IEntityStorageUriService entityStorageUriService)
        {
            AppContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _entityStorageUriService = entityStorageUriService ?? throw new ArgumentNullException(nameof(entityStorageUriService));
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
        /// <remarks>
        ///     Note that no profile image details are required here
        ///     since this is only called by our vlog trigger service.
        ///     This might change in the future though.
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vloggable user collection</returns>
        public IAsyncEnumerable<User> GetAllVloggableUsersAsync(Navigation navigation) 
            => _userRepository.GetAllVloggableUsersAsync(navigation);

        /// <summary>
        ///     Get a single user from our data store.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The user.</returns>
        public async Task<User> GetAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);

            user.ProfileImageUri = await _entityStorageUriService.GetUserProfileImageAccessUriOrNullAsync(user);

            return user;
        }

        /// <summary>
        ///     Gets all followers for a user.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All followers.</returns>
        public async IAsyncEnumerable<User> GetFollowersAsync(Guid userId, Navigation navigation)
        {
            await foreach (var user in _userRepository.GetFollowersAsync(userId, navigation))
            {
                user.ProfileImageUri = await _entityStorageUriService.GetUserProfileImageAccessUriOrNullAsync(user);

                yield return user;
            }
        }

        /// <summary>
        ///     Gets all users a user is following.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All users followed by <paramref name="userId"/>.</returns>
        public async IAsyncEnumerable<User> GetFollowingAsync(Guid userId, Navigation navigation)
        {
            await foreach (var user in _userRepository.GetFollowingAsync(userId, navigation))
            {
                user.ProfileImageUri = await _entityStorageUriService.GetUserProfileImageAccessUriOrNullAsync(user);

                yield return user;
            }
        }

        /// <summary>
        ///     Gets the details required to send a push notification.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <returns>Push notification details.</returns>
        public Task<UserPushNotificationDetails> GetUserPushDetailsAsync(Guid userId) 
            => _userRepository.GetPushDetailsAsync(userId);

        /// <summary>
        ///     Gets all <see cref="UserWithRelationWrapper"/> objects that 
        ///     belong to the users which have pending follow requests for 
        ///     the current user.
        /// </summary>
        /// <param name="navigation">Result set control.</param>
        /// <returns>Wrappers around all users that liked saids vlogs.</returns>
        public async IAsyncEnumerable<UserWithRelationWrapper> GetFollowRequestingUsersAsync(Navigation navigation)
        {
            await foreach (var userWrapper in _userRepository.GetFollowRequestingUsersAsync(navigation))
            {
                userWrapper.User.ProfileImageUri = await _entityStorageUriService.GetUserProfileImageAccessUriOrNullAsync(userWrapper.User);

                yield return userWrapper;
            }
        }

        /// <summary>
        ///     Gets all <see cref="VlogLikingUserWrapper"/> objects that 
        ///     belong to the vlogs of a given <paramref name="userId"/>.
        /// </summary>
        /// <remarks>
        ///     The user id is extracted from the <see cref="AppContext"/>.
        /// </remarks>
        /// <param name="navigation">Result set control.</param>
        /// <returns>Wrappers around all users that liked saids vlogs.</returns>
        public async IAsyncEnumerable<VlogLikingUserWrapper> GetVlogLikingUsersForUserAsync(Navigation navigation)
        {
            await foreach (var userWrapper in _userRepository.GetVlogLikingUsersForUserAsync(navigation))
            {
                userWrapper.User.ProfileImageUri = await _entityStorageUriService.GetUserProfileImageAccessUriOrNullAsync(userWrapper.User);

                yield return userWrapper;
            }
        }

        /// <summary>
        ///     Gets a user with corresponding statistics.
        /// </summary>
        /// <param name="userId">The internal user id.</param>
        /// <returns>User entity with statistics.</returns>
        public async Task<UserWithStats> GetWithStatisticsAsync(Guid userId)
        {
            var user = await _userRepository.GetWithStatisticsAsync(userId);
            
            user.ProfileImageUri = await _entityStorageUriService.GetUserProfileImageAccessUriOrNullAsync(user);

            return user;
        }

        /// <summary>
        ///     Search for users in our data store.
        /// </summary>
        /// <param name="query">Search string.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>User search result set.</returns>
        public async IAsyncEnumerable<UserWithRelationWrapper> SearchAsync(string query, Navigation navigation)
        { 
            await foreach (var userWrapper in _userRepository.SearchAsync(query, navigation))
            {
                userWrapper.User.ProfileImageUri = await _entityStorageUriService.GetUserProfileImageAccessUriOrNullAsync(userWrapper.User);

                yield return userWrapper;
            }
        }

        // TODO Validate the uploaded profile image if one was modified or added.
        /// <summary>
        ///     This updates the current user in our database.
        /// </summary>
        /// <param name="user">The updated user entity.</param>
        /// <returns>The user entity after the update operation.</returns>
        public Task UpdateAsync(User user)
            => _userRepository.UpdateAsync(user);
    }
}
