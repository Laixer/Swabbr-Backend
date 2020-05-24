using System;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Wrapper object that contains information about a livestream timeout.
    /// </summary>
    public sealed class LivestreamTimeoutWrapper
    {

        public Guid LivestreamId { get; set; }

        public Guid UserId { get; set; }

        public Guid RequestId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public TimeSpan TimeOut { get; set; }

    }

}
