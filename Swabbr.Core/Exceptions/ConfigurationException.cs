using System;

namespace Swabbr.Core.Exceptions
{
    /// <summary>
    ///     Indicates our configuration is invalid.
    /// </summary>
    public class ConfigurationException : SwabbrCoreException
    {

        public ConfigurationException(string message) : base(message)
        {
        }

        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConfigurationException()
        {
        }
    }
}
