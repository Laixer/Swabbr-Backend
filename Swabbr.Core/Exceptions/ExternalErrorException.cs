using System;

namespace Swabbr.Core.Exceptions
{
    public class ExternalErrorException : Exception
    {
        public ExternalErrorException(string message) : base(message)
        {
        }
    }
}
