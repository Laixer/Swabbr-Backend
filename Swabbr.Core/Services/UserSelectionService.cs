using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Data.HashFunction.MurmurHash;
using System.Text;

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Handles our hash distribution for user selection.
    /// </summary>
    public class UserSelectionService : IUserSelectionService
    {
        protected readonly IUserService _userService;
        protected readonly SwabbrConfiguration _options;
        protected static readonly IMurmurHash3 hasher = MurmurHash3Factory.Instance.Create();
        protected static readonly UTF8Encoding encoder = new UTF8Encoding();

        public uint ValidMinutes => _options.VlogRequestEndTimeMinutes - _options.VlogRequestStartTimeMinutes;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public UserSelectionService(IUserService userService,
            IOptions<SwabbrConfiguration> options)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        ///     Uses hash distribution to get all users which should 
        ///     receive a request at a specified minute of the day.
        /// </summary>
        /// <remarks>
        ///     This will use a hash function to calculate all the minutes
        ///     in a day for each user on which that user should receive a
        ///     vlog record request. All users for which the specified 
        ///     <paramref name="time"/> minute matches any of their calculated
        ///     minutes get returned. This also takes timezones into account.
        /// </remarks>
        /// <param name="time"><see cref="DateTimeOffset"/></param>
        /// <param name="offset"><see cref="TimeSpan"/></param>
        /// <returns><see cref="User"/> collection</returns>
        public virtual async IAsyncEnumerable<User> GetForMinuteAsync(DateTimeOffset time, TimeSpan? offset = null)
        {
            // Extract the current minute from the time parameter.
            var day = new DateTime(time.Year, time.Month, time.Day);
            var timeUtc = time.ToUniversalTime() + (offset ?? TimeSpan.Zero);
            var thisMinute = GetMinutes(timeUtc);

            // This call gets all users which are eligible for a vlog request,
            // meaning they have a vlog request limit greater than zero.
            await foreach (var user in _userService.GetAllVloggableUsersAsync(Navigation.All))
            {
                // Perform one check for each possible vlog request,
                // since users can have more than one request per day.
                for (int i = 1; i <= Math.Min(_options.MaxDailyVlogRequestLimit, user.DailyVlogRequestLimit); i++)
                {
                    // Get the matching minute on which a user should receive 
                    // a request and correct it using the users timezone.
                    var userMinute = GetHashMinute(user, day, i);
                    var userMinuteCorrected = userMinute - user.Timezone.BaseUtcOffset.TotalMinutes;
                    if (userMinuteCorrected < 0) { userMinuteCorrected += 60 * 24; }

                    if (userMinuteCorrected == thisMinute)
                    {
                        yield return user;
                    }
                }
            }
        }

        /// <summary>
        ///     Hashes a single user into a corresponding minute
        ///     of the day, represented as <see cref="int"/> value.
        /// </summary>
        /// <remarks>
        ///     This algorithm computes a Murmur3 hash of a composed string value which contains:
        ///         - The user id
        ///         - The day
        ///         - The request index, ranging between 1 and the vlog request limit for that user
        ///     The computed hash is then converted to a uint32. The resulting minute is computed
        ///     by taking the <see cref="SwabbrConfiguration.VlogRequestStartTimeMinutes"/> plus
        ///     the uint32 modulo <see cref="SwabbrConfiguration.VlogRequestEndTimeMinutes"/>.
        /// 
        ///     This ignores the <see cref="User.Timezone"/> value.
        /// </remarks>
        /// <param name="user"><see cref="User"/></param>
        /// <param name="day"><see cref="DateTime"/></param>
        /// <param name="requestIndex">Index of the request on the day</param>
        /// <returns>The minute in the day based on the inputs</returns>
        protected virtual int GetHashMinute(User user, DateTime day, int requestIndex)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            if (user.Id.IsEmpty()) { throw new ArgumentNullException(nameof(user.Id)); }
            if (user.Timezone == null) { throw new ArgumentNullException(nameof(user.Timezone)); }

            var hashString = $"{user.Id}{day.Year}{day.Month}{day.Day}{requestIndex}";
            var hash = hasher.ComputeHash(encoder.GetBytes(hashString));

            // TODO Make sure we never exceed a byte[] length of 4
            var number = BitConverter.ToUInt32(hash.Hash, 0);
            // In theory this can overflow, but in practise we ALWAYS shrink this down to somewhere within 24*60 minutes
            var minute = _options.VlogRequestStartTimeMinutes + (number % ValidMinutes);
            return (int)Math.Abs(minute);
        }

        // TODO Move to helper
        /// <summary>
        ///     Extracts the minutes from a date time offset.
        /// </summary>
        /// <param name="date">The object to extract from.</param>
        /// <returns>The amount of minutes in the object.</returns>
        private static int GetMinutes(DateTimeOffset date)
            => date.Hour * 60 + date.Minute;
    }
}
