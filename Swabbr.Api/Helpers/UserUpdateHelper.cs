using Swabbr.Api.DataTransferObjects;
using Swabbr.Core.Abstractions;
using Swabbr.Core.Helpers;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Helpers
{
    /// <summary>
    ///     This class contains functionality to ensure that
    ///     whenever we update a user, only the assigned 
    ///     properties will be updated.
    /// </summary>
    /// <remarks>
    ///     This design decision was made because we have a lot
    ///     of different calls for updating user properties. Next
    ///     to the user details we also have timezone and location 
    ///     updates.
    ///     To ensure nothing gets set to its default value this 
    ///     user update helper class can be called whenever any of
    ///     these properties is updated.
    /// </remarks>
    public class UserUpdateHelper : AppServiceBase
    {
        private readonly IUserService _userService;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public UserUpdateHelper(Core.AppContext appContext,
            IUserService userService)
        {
            AppContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // TODO This is a tempfix.
        /// <summary>
        ///     Update a user in our datastore, only modifying
        ///     the explicitly assigned properties in the input.
        /// </summary>
        /// <remarks>
        ///     Any property that is null (default) will be untouched.
        /// </remarks>
        /// <param name="input">User with explicitly assigned properties.</param>
        /// <param name="userId">User id to assign to AppContext.</param>
        internal Task UpdateUserAsync(UserUpdateDto input, Guid userId)
        {
            // TODO: This will be removed when refactoring the auth part. Currently when the
            //         AuthenticationController calls this function, a problem is created where 
            //         we want to update the user without being logged in as that user. This 
            //         means the AppContext has no UserId assigned to it, while the UserRepository 
            //         expects one. As a tempfix we explicitly assign the user id to the AppContext. 
            //         This has to be fixed 100% but the current solution prevents us from refactoring
            //         a bunch of code while the real problem is the authentication of the app, which
            //         will be refactored anyways. See issue #217 https://github.com/Laixer/Swabbr-Backend/issues/217
            AppContext.UserId = userId;

            return UpdateUserAsync(input);
        }

        /// <summary>
        ///     Update a user in our datastore, only modifying
        ///     the explicitly assigned properties in the input.
        /// </summary>
        /// <remarks>
        ///     Any property that is null (default) will be untouched.
        /// </remarks>
        /// <param name="input">User with explicitly assigned properties.</param>
        internal async Task UpdateUserAsync(UserUpdateDto input)
        { 
            // First get the current user from our data store.
            var currentUser = await _userService.GetAsync(AppContext.UserId);

            // Only re-assign properties if they were assigned explicitly.
            currentUser.BirthDate = input.BirthDate ?? currentUser.BirthDate;
            currentUser.Country = input.Country ?? currentUser.Country;
            currentUser.DailyVlogRequestLimit = input.DailyVlogRequestLimit ?? currentUser.DailyVlogRequestLimit;
            currentUser.FirstName = input.FirstName ?? currentUser.FirstName;
            currentUser.FollowMode = input.FollowMode ?? currentUser.FollowMode;
            currentUser.Gender = input.Gender ?? currentUser.Gender;
            currentUser.IsPrivate = input.IsPrivate ?? currentUser.IsPrivate;
            currentUser.LastName = input.LastName ?? currentUser.LastName;
            currentUser.Latitude = input.Latitude ?? currentUser.Latitude;
            currentUser.Longitude = input.Longitude ?? currentUser.Longitude;
            currentUser.Nickname = input.Nickname ?? currentUser.Nickname;
            currentUser.ProfileImageBase64Encoded = input.ProfileImageBase64Encoded ?? currentUser.ProfileImageBase64Encoded;
            currentUser.TimeZone = (string.IsNullOrEmpty(input.TimeZone))
                ? currentUser.TimeZone
                : TimeZoneInfoHelper.MapStringToTimeZone(input.TimeZone);

                // Update only the explicitly modified properties
                await _userService.UpdateAsync(currentUser);
        }
    }
}
