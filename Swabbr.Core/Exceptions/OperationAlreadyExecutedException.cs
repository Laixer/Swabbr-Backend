using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// <see cref="Exception"/> indicating that some operation was already performed
    /// before, thus the expected output state is already the current state.
    /// </summary>
    public sealed class OperationAlreadyExecutedException : Exception
    {

        public OperationAlreadyExecutedException(string message) : base(message) { }

        public OperationAlreadyExecutedException(string message, Exception innerException) : base(message, innerException) { }

    }

}
