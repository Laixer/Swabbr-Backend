using Swabbr.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace Swabbr.Core.Tests.Helpers
{
    /// <summary>
    ///     Tests <see cref="TimeZoneInfoHelper"/> helper functionality.
    /// </summary>
    public class TimeZoneInfoHelperTests
    {
        [Fact]
        public void MappedTimeZoneInfoMatchesRegex()
        {
            // Arrange.
            var timeZoneInfo = TimeZoneInfo.Utc;

            // Act.
            var result = TimeZoneInfoHelper.MapTimeZoneToStringOrNull(timeZoneInfo);

            // Assert.
            Assert.Matches(RegexConstants.TimeZoneRegex, result);
        }

        [Fact]
        public void NullTimeZoneMapsToNull()
        {
            // Act.
            var result = TimeZoneInfoHelper.MapTimeZoneToStringOrNull(null);

            // Assert.
            Assert.Null(result);
        }
    }
}
