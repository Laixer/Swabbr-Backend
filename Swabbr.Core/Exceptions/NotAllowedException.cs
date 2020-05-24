using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// <see cref="Exception"/> indicating we are not allowed to perform a certain
    /// action in our backend.
    /// </summary>
    public sealed class NotAllowedException : Exception
    {

        public NotAllowedException(string message) : base(message) { }

        public NotAllowedException(string message, Exception innerException) : base(message, innerException) { }

        public NotAllowedException()
        {
        }
    }

}
