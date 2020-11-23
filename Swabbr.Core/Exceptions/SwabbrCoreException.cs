using System;
using System.Runtime.Serialization;

namespace Swabbr.Core.Exceptions
{
    /// <summary>
    ///     Abstract base exception for Swabbr.
    /// </summary>
    public abstract class SwabbrCoreException : Exception
    {
        protected SwabbrCoreException()
        {
        }

        protected SwabbrCoreException(string message) : base(message)
        {
        }

        protected SwabbrCoreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected SwabbrCoreException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
