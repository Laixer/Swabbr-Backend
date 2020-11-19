using Microsoft.Extensions.Options;
using Swabbr.Core.Configuration;
using Swabbr.Core.Entities;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Utility;
using System;
using System.Collections.Generic;
using System.Data.HashFunction.MurmurHash;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Handles our hash distribution for user selection.
    /// </summary>
    public sealed class UserSelectionService : IUserSelectionService
    {
        private readonly IUserService _userService;
        private readonly SwabbrConfiguration _options;
        private static readonly IMurmurHash3 hasher = MurmurHash3Factory.Instance.Create();
        private static readonly UTF8Encoding encoder = new UTF8Encoding();

        public uint ValidMinutes => _options.VlogRequestEndTimeMinutes - _options.VlogRequestStartTimeMinutes;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserSelectionService(IUserService userService,
            IOptions<SwabbrConfiguration> options)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            if (options == null || options.Value == null) { throw new ArgumentNullException(nameof(options)); }
            options.Value.ThrowIfInvalid();
            _options = options.Value;
        }

        /// <summary>
        ///     Uses hash distribution to get all users 
        ///     which trigger in a given moment.
        /// </summary>
        /// <param name="time"><see cref="DateTimeOffset"/></param>
        /// <param name="offset"><see cref="TimeSpan"/></param>
        /// <returns><see cref="SwabbrUser"/> collection</returns>
        public async Task<IEnumerable<SwabbrUser>> GetForMinuteAsync(DateTimeOffset time, TimeSpan? offset = null)
        {
            var users = await _userService.GetAllVloggableUsersAsync().ConfigureAwait(false);

            var day = new DateTime(time.Year, time.Month, time.Day);
            var timeUtc = time.ToUniversalTime() + (offset ?? TimeSpan.Zero);
            var thisMinute = GetMinutes(timeUtc);

            var result = new List<SwabbrUser>();
            foreach (var user in users)
            {
                // Perform one check for each possible vlog request
                for (int i = 1; i <= Math.Min(_options.DailyVlogRequestLimit, user.DailyVlogRequestLimit); i++)
                {
                    var userMinute = GetHashMinute(user, day, i);
                    var userMinuteCorrected = userMinute - user.Timezone.BaseUtcOffset.TotalMinutes;
                    if (userMinuteCorrected < 0) { userMinuteCorrected += 60 * 24; }

                    if (userMinuteCorrected == thisMinute)
                    {
                        // TODO Use yield
                        result.Add(user);
                    }
                }
            }

            return result;
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
        ///     This ignores the <see cref="SwabbrUser.Timezone"/> value.
        /// </remarks>
        /// <param name="user"><see cref="SwabbrUser"/></param>
        /// <param name="day"><see cref="DateTime"/></param>
        /// <param name="requestIndex">Index of the request on the day</param>
        /// <returns>The minute in the day based on the inputs</returns>
        private int GetHashMinute(SwabbrUser user, DateTime day, int requestIndex)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            if (user.Id.IsNullOrEmpty()) { throw new ArgumentNullException(nameof(user.Id)); }
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
