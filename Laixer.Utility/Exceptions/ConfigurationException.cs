using System;

namespace Laixer.Utility.Exceptions
{

    /// <summary>
    /// Indicates our configuration is incorrect.
    /// </summary>
    public class ConfigurationException : Exception
    {

        public ConfigurationException(string message) : base(message)
        {
        }

        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
