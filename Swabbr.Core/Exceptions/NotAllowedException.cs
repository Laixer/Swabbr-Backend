using System;
using System.Runtime.Serialization;

namespace Swabbr.Core.Exceptions
{
    /// <summary>
    ///     Indicates a backend operation which is not allowed
    ///     based on our user identity.
    /// </summary>
    public sealed class NotAllowedException : SwabbrCoreException
    {
        public NotAllowedException()
        {
        }

        public NotAllowedException(string message) : base(message)
        {
        }

        public NotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NotAllowedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
