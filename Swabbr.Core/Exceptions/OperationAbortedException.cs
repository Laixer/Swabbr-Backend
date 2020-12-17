using System;
using System.Runtime.Serialization;

namespace Swabbr.Core.Exceptions
{
    /// <summary>
    ///     Exception indicating some operation was cancelled.
    /// </summary>
    /// <remarks>
    ///     This is called Aborted instead of Canceled because
    ///     there already exists an OperationCancelledException.
    ///     By using this custom cancellation exception we are
    ///     able to catch and handle it explicitly.
    /// </remarks>
    public class OperationAbortedException : SwabbrCoreException
    {
        public OperationAbortedException()
        {
        }

        public OperationAbortedException(string message) : base(message)
        {
        }

        public OperationAbortedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public OperationAbortedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
