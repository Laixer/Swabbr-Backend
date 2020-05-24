using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// Indicates the <see cref="Entities.Livestream"/> status is invalid for
    /// a given operation.
    /// </summary>
    public sealed class LivestreamStateException : Exception
    {

        public LivestreamStateException() { }

        public LivestreamStateException(string message) : base(message) { }

        public LivestreamStateException(string message, Exception innerException) : base(message, innerException) { }

    }

}
