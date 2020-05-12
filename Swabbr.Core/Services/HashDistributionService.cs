using Laixer.Utility.Exceptions;
using Microsoft.Extensions.Options;
using Swabbr.Core.Configuration;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.HashFunction.MurmurHash;
using System.Text;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Handles our hash distribution.
    /// </summary>
    public sealed class HashDistributionService : IHashDistributionService    {

        private readonly SwabbrConfiguration config;
        private static readonly IMurmurHash3 hasher = MurmurHash3Factory.Instance.Create();
        private static readonly UTF8Encoding encoder = new UTF8Encoding();

        private static int validMinutes;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public HashDistributionService(IOptions<SwabbrConfiguration> options)
        {
            if (options == null || options.Value == null) { throw new ArgumentNullException(nameof(options)); }
            options.Value.ThrowIfInvalid();
            config = options.Value;
            validMinutes = config.VlogRequestEndTimeMinutes - config.VlogRequestStartTimeMinutes;
        }

        /// <summary>
        /// Uses hash distribution to get all users which trigger in a given moment.
        /// </summary>
        /// <param name="users"><see cref="SwabbrUserMinified"/></param>
        /// <param name="time"><see cref="DateTimeOffset"/></param>
        /// <param name="offset"><see cref="TimeSpan"/></param>
        /// <returns><see cref="SwabbrUserMinified"/> collection</returns>
        public IEnumerable<SwabbrUserMinified> GetForMinute(IEnumerable<SwabbrUserMinified> users, DateTimeOffset time, TimeSpan? offset = null)
        {
            if (users == null) { throw new ArgumentNullException(nameof(users)); }
            if (time == null) { throw new ArgumentNullException(nameof(time)); }

            var day = new DateTime(time.Year, time.Month, time.Day);
            var timeUtc = time.ToUniversalTime() + (offset ?? TimeSpan.Zero);
            var thisMinute = GetMinutes(timeUtc);

            var result = new Collection<SwabbrUserMinified>();
            foreach (var user in users)
            {
                // Perform one check for each possible vlog request
                for (int i = 1; i <= Math.Min(config.DailyVlogRequestLimit, user.DailyVlogRequestLimit); i++)
                {
                    var userMinute = GetHashMinute(user, day, i);
                    var userMinuteCorrected = userMinute - user.TimeZone.BaseUtcOffset.TotalMinutes;
                    if (userMinuteCorrected < 0) { userMinuteCorrected += 60 * 24; }

                    if (userMinuteCorrected == thisMinute) { result.Add(user); }
                }
            }

            if (result.Count > 0) { }

            return result;
        }

        // TODO Debug, remove this
        public IEnumerable<KeyValuePair<SwabbrUserMinified, int>> DebugGetAllForDate(IEnumerable<SwabbrUserMinified> users, DateTimeOffset time, TimeSpan? offset = null)
        {
            if (users == null) { throw new ArgumentNullException(nameof(users)); }
            if (time == null) { throw new ArgumentNullException(nameof(time)); }

            var day = new DateTime(time.Year, time.Month, time.Day);

            var result = new Collection<KeyValuePair<SwabbrUserMinified, int>>();
            foreach (var user in users)
            {
                for (int i = 1; i <= Math.Min(config.DailyVlogRequestLimit, user.DailyVlogRequestLimit); i++)
                {
                    var userMinute = GetHashMinute(user, day, i);
                    result.Add(new KeyValuePair<SwabbrUserMinified, int>(user, userMinute));
                }
            }

            return result;
        }

        /// <summary>
        /// Hashes a single <see cref="SwabbrUserMinified"/>.
        /// </summary>
        /// <remarks>
        /// This ignores the <see cref="SwabbrUserMinified.TimeZone"/> value.
        /// </remarks>
        /// <param name="user"><see cref="SwabbrUserMinified"/></param>
        /// <returns></returns>
        private int GetHashMinute(SwabbrUserMinified user, DateTime day, int requestIndex)
        {
            var hashString = $"{user.Id}{day.Year}{day.Month}{day.Day}{requestIndex}";
            var hash = hasher.ComputeHash(encoder.GetBytes(hashString));

            // TODO In theory this can overflow, but in practise we ALWAYS shrink this down to somewhere within 24*60 minutes
            var number = BitConverter.ToUInt32(hash.Hash, 0);
            var minute = config.VlogRequestStartTimeMinutes + (number % validMinutes);
            return (int)minute;
        }

        private int GetMinutes(DateTimeOffset date)
        {
            return date.Hour * 60 + date.Minute;
        }
    }
}
