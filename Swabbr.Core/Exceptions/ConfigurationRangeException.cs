using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// Indicates our configuration value is out of range.
    /// </summary>
    public class ConfigurationRangeException : ConfigurationException
    {
        public ConfigurationRangeException(string message) : base(message)
        {
        }

        public ConfigurationRangeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConfigurationRangeException()
        {
        }
    }

}
