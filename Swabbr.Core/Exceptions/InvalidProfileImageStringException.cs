using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// Indicates an invalid profile image base64 encoded string.
    /// TODO Too specific
    /// </summary>
    public sealed class InvalidProfileImageStringException : Exception
    {

        public InvalidProfileImageStringException() { }

        public InvalidProfileImageStringException(string message)
            : base(message) { }

        public InvalidProfileImageStringException(string message, Exception innerException)
            : base(message, innerException) { }

    }

}
