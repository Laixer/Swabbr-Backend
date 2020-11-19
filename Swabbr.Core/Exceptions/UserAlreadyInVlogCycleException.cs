using System;

namespace Swabbr.Core.Exceptions
{
    /// <summary>
    ///     Represents the situation where a user already is in a
    ///     vlog recording cycle when a second one is started.
    /// </summary>
    public sealed class UserAlreadyInVlogCycleException : Exception
    {
        public UserAlreadyInVlogCycleException() { }

        public UserAlreadyInVlogCycleException(string message)
            : base(message) { }

        public UserAlreadyInVlogCycleException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
