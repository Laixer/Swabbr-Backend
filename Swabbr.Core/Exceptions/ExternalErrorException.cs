using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// <see cref="Exception"/> indicating an external service threw an exception.
    /// </summary>
    public class ExternalErrorException : Exception
    {

        public ExternalErrorException() { }

        public ExternalErrorException(string message) 
            : base(message) { }

        public ExternalErrorException(string message, Exception innerException) 
            : base(message, innerException) { }


    }

}
