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

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Handles our hash distribution for user selection.
    /// </summary>
    public sealed class HashDistributionService : IHashDistributionService
    {
        private readonly SwabbrConfiguration config;
        private static readonly IMurmurHash3 hasher = MurmurHash3Factory.Instance.Create();
        private static readonly UTF8Encoding encoder = new UTF8Encoding();

        public uint ValidMinutes => config.VlogRequestEndTimeMinutes - config.VlogRequestStartTimeMinutes;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public HashDistributionService(IOptions<SwabbrConfiguration> options)
        {
            if (options == null || options.Value == null) { throw new ArgumentNullException(nameof(options)); }
            options.Value.ThrowIfInvalid();
            config = options.Value;
        }

        /// <summary>
        /// Uses hash distribution to get all users which trigger in a given moment.
        /// </summary>
        /// <param name="users"><see cref="SwabbrUser"/></param>
        /// <param name="time"><see cref="DateTimeOffset"/></param>
        /// <param name="offset"><see cref="TimeSpan"/></param>
        /// <returns><see cref="SwabbrUser"/> collection</returns>
        public IEnumerable<SwabbrUser> GetForMinute(IEnumerable<SwabbrUser> users, DateTimeOffset time, TimeSpan? offset = null)
        {
            if (users == null) { throw new ArgumentNullException(nameof(users)); }
            if (time == null) { throw new ArgumentNullException(nameof(time)); }

            var day = new DateTime(time.Year, time.Month, time.Day);
            var timeUtc = time.ToUniversalTime() + (offset ?? TimeSpan.Zero);
            var thisMinute = GetMinutes(timeUtc);

            var result = new List<SwabbrUser>();
            foreach (var user in users)
            {
                // Perform one check for each possible vlog request
                for (int i = 1; i <= Math.Min(config.DailyVlogRequestLimit, user.DailyVlogRequestLimit); i++)
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
        /// Hashes a single <see cref="SwabbrUser"/> into a corresponding minute
        /// of the day, represented as <see cref="int"/> value.
        /// </summary>
        /// <remarks>
        /// This algorithm computes a Murmur3 hash of a composed string value which contains:
        ///     - The user id
        ///     - The day
        ///     - The request index, ranging between 1 and the vlog request limit for that user
        /// The computed hash is then converted to a uint32. The resulting minute is computed
        /// by taking the <see cref="SwabbrConfiguration.VlogRequestStartTimeMinutes"/> plus
        /// the uint32 modulo <see cref="SwabbrConfiguration.VlogRequestEndTimeMinutes"/>.
        /// 
        /// This ignores the <see cref="SwabbrUser.Timezone"/> value.
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
            var minute = config.VlogRequestStartTimeMinutes + (number % ValidMinutes);
            return (int)Math.Abs(minute);
        }

        private static int GetMinutes(DateTimeOffset date)
        {
            if (date.IsNullOrEmpty()) { throw new ArgumentNullException(nameof(date)); }
            return date.Hour * 60 + date.Minute;
        }
    }
}
