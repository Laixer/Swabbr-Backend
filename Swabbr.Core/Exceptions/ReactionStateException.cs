using System;
using System.Runtime.Serialization;

namespace Swabbr.Core.Exceptions
{
    /// <summary>
    ///     Indicates the reaction status is invalid for
    ///     a given operation.
    /// </summary>
    public sealed class ReactionStateException : Exception
    {
        public ReactionStateException() 
        { 
        }

        public ReactionStateException(string message) : base(message) 
        {
        }

        public ReactionStateException(string message, Exception innerException) : base(message, innerException) 
        { 
        }

        public ReactionStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
