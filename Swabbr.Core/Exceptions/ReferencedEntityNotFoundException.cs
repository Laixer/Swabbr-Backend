using System;
using System.Runtime.Serialization;

namespace Swabbr.Core.Exceptions
{
    /// <summary>
    ///     Indicates a reference to some other entity
    ///     which does not exist in our data store.
    /// </summary>
    public class ReferencedEntityNotFoundException : SwabbrCoreException
    {
        public ReferencedEntityNotFoundException()
        {
        }

        public ReferencedEntityNotFoundException(string message) : base(message)
        {
        }

        public ReferencedEntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ReferencedEntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
