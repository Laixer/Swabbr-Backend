using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
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
        private readonly IUserService _userService;
        private readonly SwabbrConfiguration _options;
        private static readonly IMurmurHash3 hasher = MurmurHash3Factory.Instance.Create();
        private static readonly UTF8Encoding encoder = new UTF8Encoding();

        /// <summary>
        ///     We enforce a minimum request interval of 60 minutes.
        /// </summary>
        private static readonly int interval = 60;

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
            var thisMinute = (timeUtc.Hour * 60) + timeUtc.Minute;

            // This call gets all users which are eligible for a vlog request,
            // meaning they have a vlog request limit greater than zero.
            await foreach (var user in _userService.GetAllVloggableUsersAsync(Navigation.All))
            {
                var requestCount = Math.Min(_options.MaxDailyVlogRequestLimit, user.DailyVlogRequestLimit);

                if (requestCount <= 0)
                {
                    continue;
                }

                // We store each discovered minute to compare them to each other, 
                // making sure a minimum interval exists between them. The minimum 
                // interval is only checked BEFORE the current minute, meaning the 
                // first one of the two is still selected. The latter of the two 
                // will not be selected by this algorithm.
                var minutes = new double[requestCount];

                // Perform one check for each possible vlog request,
                // since users can have more than one request per day.
                for (uint i = 1; i <= Math.Min(_options.MaxDailyVlogRequestLimit, user.DailyVlogRequestLimit); i++)
                {
                    // Get the matching minute on which a user should receive 
                    // a request and correct it using the users timezone.
                    var userMinute = GetHashMinute(user, day, i);
                    var userMinuteCorrected = userMinute - user.TimeZone.BaseUtcOffset.TotalMinutes;
                    if (userMinuteCorrected < 0) { userMinuteCorrected += 60 * 24; }

                    minutes[i - 1] = userMinuteCorrected;
                }

                for (int i = 0; i < requestCount; i++)
                {
                    // If any of our selected minutes matches the current one, continue with it.
                    // Compare it to the other minutes to ensure a minimum interval between them.
                    if (minutes[i] == thisMinute)
                    {
                        var withinMinimumInterval = false;

                        for (int j = 0; j < requestCount; j++)
                        {
                            var delta = minutes[i] - minutes[j];
                            if (j != i && delta < interval && delta >= 0)
                            {
                                withinMinimumInterval = true;
                            }
                        }

                        // Only return the user if none of the other minutes were
                        // within (before) the interval of the currently selected minute.
                        if (!withinMinimumInterval)
                        {
                            yield return user;
                        }
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
        ///     This ignores the <see cref="User.TimeZone"/> value.
        /// </remarks>
        /// <param name="user"><see cref="User"/></param>
        /// <param name="day"><see cref="DateTime"/></param>
        /// <param name="requestIndex">Index of the request on the day</param>
        /// <returns>The minute in the day based on the inputs</returns>
        protected int GetHashMinute(User user, DateTime day, uint requestIndex)
        {
            if (user is null) 
            {
                throw new ArgumentNullException(nameof(user)); 
            }
            if (user.Id == Guid.Empty)
            {
                throw new InvalidOperationException();
            }
            if (user.TimeZone is null)
            {
                throw new InvalidOperationException();
            }

            var hashString = $"{user.Id}{day.Year}{day.Month}{day.Day}{requestIndex}";
            var hash = hasher.ComputeHash(encoder.GetBytes(hashString));

            var number = BitConverter.ToUInt32(hash.Hash, 0);

            // Note: In theory this can overflow, but in practise we will 
            //       shrink this down to somewhere within 24*60 minutes
            //       as long as our configuration is setup correctly.
            var minute = _options.VlogRequestStartTimeMinutes + (number % (_options.VlogRequestEndTimeMinutes - _options.VlogRequestStartTimeMinutes));

            return (int)Math.Abs(minute);
        }
    }
}
