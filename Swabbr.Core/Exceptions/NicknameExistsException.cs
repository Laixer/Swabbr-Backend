using System;
using System.Runtime.Serialization;

namespace Swabbr.Core.Exceptions
{
    /// <summary>
    ///     Indicates a given nickname already exists.
    /// </summary>
    public sealed class NicknameExistsException : SwabbrCoreException
    {
        public NicknameExistsException()
        {
        }

        public NicknameExistsException(string message) : base(message)
        {
        }

        public NicknameExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NicknameExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

