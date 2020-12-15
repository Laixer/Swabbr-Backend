using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Interfaces
{
    /// <summary>
    ///     Contract for mapping our exceptions.
    /// </summary>
    /// <typeparam name="TException">Exception type to map.</typeparam>
    public interface IExceptionMapper<TException>
        where TException : Exception
    {
        /// <summary>
        ///     Converts an exception to an error message.
        /// </summary>
        /// <param name="exception">The thrown exception.</param>
        /// <returns>Error message.</returns>
        public ErrorMessage Map(TException exception);
    }
}
