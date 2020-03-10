using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// Indicates that a user was not found in our backend.
    /// </summary>
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
        {
        }

        public UserNotFoundException(string message) : base(message)
        {
        }

        public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}