using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// Indicates we have no available <see cref="Entities.Livestream"/>.
    /// </summary>
    public sealed class NoLivestreamAvailableException : Exception
    {

        public NoLivestreamAvailableException() { }

        public NoLivestreamAvailableException(string message) : base(message) { }

        public NoLivestreamAvailableException(string message, Exception innerException) : base(message, innerException) { }

    }

}
