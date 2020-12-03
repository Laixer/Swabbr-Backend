using System;
using System.Runtime.Serialization;

namespace Swabbr.Core.Exceptions
{
    /// <summary>
    ///     Indicates an invalid profile image base64 encoded string.
    /// </summary>
    public sealed class InvalidProfileImageStringException : SwabbrCoreException
    {
        public InvalidProfileImageStringException()
        {
        }

        public InvalidProfileImageStringException(string message) : base(message)
        {
        }

        public InvalidProfileImageStringException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public InvalidProfileImageStringException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
