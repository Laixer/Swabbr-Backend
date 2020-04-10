using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// Indicates a given operation occured outside of the permitted vlog request period.
    /// </summary>
    public sealed class OutsideVlogRequestPeriodException : Exception
    {
        public OutsideVlogRequestPeriodException()
        {
        }

        public OutsideVlogRequestPeriodException(string message) : base(message)
        {
        }

        public OutsideVlogRequestPeriodException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }

}
