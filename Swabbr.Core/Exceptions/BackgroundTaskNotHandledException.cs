using System;
using System.Runtime.Serialization;

namespace Swabbr.Core.Exceptions
{
    /// <summary>
    ///     Indicates none of our background tasks were able to
    ///     process a given request.
    /// </summary>
    public class BackgroundTaskNotHandledException : SwabbrCoreException
    {
        public BackgroundTaskNotHandledException()
        {
        }

        public BackgroundTaskNotHandledException(string message) : base(message)
        {
        }

        public BackgroundTaskNotHandledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public BackgroundTaskNotHandledException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
