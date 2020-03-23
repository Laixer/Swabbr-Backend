using System;
using System.Runtime.Serialization;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// Indicates we found more than one <see cref="Entities.EntityBase{TPrimary}"/> 
    /// in our data store, where we should only find one.
    /// </summary>
    public sealed class MultipleEntitiesFoundException : Exception
    {
        public MultipleEntitiesFoundException(string message) : base(message)
        {
        }

        public MultipleEntitiesFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MultipleEntitiesFoundException()
        {
        }

        protected MultipleEntitiesFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }

}
