using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// Indicates a <see cref="Entities.SwabbrUser"/> does not own a given
    /// <see cref="Entities.EntityBase{TPrimary}"/>.
    /// </summary>
    public sealed class UserNotOwnerException : Exception
    {

        public UserNotOwnerException() { }

        public UserNotOwnerException(string message) : base(message) { }

        public UserNotOwnerException(string message, Exception innerException) : base(message, innerException) { }

    }

}

