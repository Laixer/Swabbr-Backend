using System;
using System.Collections.Generic;
using System.Text;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// Indicates a given <see cref="Entities.SwabbrUser.Nickname"/> already exists.
    /// </summary>
    public sealed class NicknameExistsException : Exception
    {

        public NicknameExistsException() { }

        public NicknameExistsException(string message)
            : base(message) { }

        public NicknameExistsException(string message, Exception innerException)
            : base(message, innerException) { }

    }

}

