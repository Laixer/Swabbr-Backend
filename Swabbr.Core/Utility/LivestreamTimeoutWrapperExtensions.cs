using Laixer.Utility.Extensions;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Utility
{

    /// <summary>
    /// Contains extension functionality for <see cref="LivestreamTimeoutWrapper"/>.
    /// </summary>
    public static class LivestreamTimeoutWrapperExtensions
    {

        /// <summary>
        /// Throws if a <paramref name="wrapper"/> is <see cref="null"/> or contains
        /// incomplete information.
        /// </summary>
        /// <param name="wrapper"><see cref="LivestreamTimeoutWrapper"/></param>
        public static void ThrowIfInvalid(this LivestreamTimeoutWrapper wrapper)
        {
            if (wrapper == null) { throw new ArgumentNullException(nameof(wrapper)); }
            wrapper.LivestreamId.ThrowIfNullOrEmpty();
            wrapper.RequestId.ThrowIfNullOrEmpty();
            if (wrapper.StartDate == null) { throw new ArgumentNullException(nameof(wrapper.StartDate)); }
            if (wrapper.TimeOut == null) { throw new ArgumentNullException(nameof(wrapper.TimeOut)); }
            wrapper.UserId.ThrowIfNullOrEmpty();
        }

    }

}
