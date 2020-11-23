using System;
using System.Runtime.Serialization;

namespace Swabbr.Core.Exceptions
{
    /// <summary>
    ///     Indicates an exception with regards to storage.
    /// </summary>
    public class StorageException : SwabbrCoreException
    {
        public StorageException()
        {
        }

        public StorageException(string message) : base(message)
        {
        }

        public StorageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public StorageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
