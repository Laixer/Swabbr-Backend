﻿using System;

namespace Laixer.Utility.Exceptions
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

    }

}
