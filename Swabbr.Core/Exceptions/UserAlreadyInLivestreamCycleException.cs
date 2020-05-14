using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// Represents the situation where a <see cref="Entities.SwabbrUser"/> is
    /// in a livestreaming cycle when a second one is started.
    /// </summary>
    public sealed class UserAlreadyInLivestreamCycleException : Exception
    {

        public UserAlreadyInLivestreamCycleException() { }

        public UserAlreadyInLivestreamCycleException(string message)
            : base(message) { }

        public UserAlreadyInLivestreamCycleException(string message, Exception innerException)
            : base(message, innerException) { }

    }

}
