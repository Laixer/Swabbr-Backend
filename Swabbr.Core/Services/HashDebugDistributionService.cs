using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// TODO Debug remove
    /// This exists for testing purposes only.
    /// </summary>
    public sealed class HashDebugDistributionService : IHashDistributionService
    {

        private static DateTimeOffset? firstMinute = null;
        private static readonly object lockObject = new object();

        public IEnumerable<SwabbrUserMinified> GetForMinute(IEnumerable<SwabbrUserMinified> users, DateTimeOffset time, TimeSpan? offset = null)
        {
            var asMinute = new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0, TimeSpan.Zero);
            lock (lockObject)
            {
                if (firstMinute == null)
                {
                    firstMinute = asMinute;
                }
            }

            var totalMinutes = firstMinute?.Hour * 60 + firstMinute?.Minute;
            var thisMinutes = time.Hour * 60 + time.Minute - ((offset == null) ? 0 : (int)offset?.TotalMinutes);

            if (totalMinutes == thisMinutes)
            {
                foreach (var user in users)
                {
                    if (user.Id == new Guid("e2c8b3f3-6882-4d12-bfcf-ac46b1b3d2ee"))
                    {
                        return new Collection<SwabbrUserMinified> { user };
                    }
                }
                return new Collection<SwabbrUserMinified>();
            }
            else
            {
                return new Collection<SwabbrUserMinified>();
            }
        }

    }

}
