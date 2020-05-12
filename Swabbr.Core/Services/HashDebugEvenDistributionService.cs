using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Returns the user each <see cref="MinutesInterval"/> minutes.
    /// </summary>
    public sealed class HashDebugEvenDistributionService : IHashDistributionService
    {

        private const int MinutesInterval = 6;

        public IEnumerable<SwabbrUserMinified> GetForMinute(IEnumerable<SwabbrUserMinified> users, DateTimeOffset time, TimeSpan? offset = null)
        {
            if (users == null) { throw new ArgumentNullException(nameof(users)); }
            time.ThrowIfNullOrEmpty();

            time.Subtract(offset ?? TimeSpan.Zero);
            var minute = time.GetMinutes();

            if (minute % MinutesInterval == 0)
            {
                return new List<SwabbrUserMinified> { new SwabbrUserMinified
                {
                    DailyVlogRequestLimit = 3,
                    Id = new Guid("e2c8b3f3-6882-4d12-bfcf-ac46b1b3d2ee"),
                    TimeZone = TimeZoneInfo.Utc
                } };
            }
            else
            {
                return new List<SwabbrUserMinified>();
            }
        }
    }

}
