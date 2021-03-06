﻿using System;
using System.Runtime.Serialization;

namespace Swabbr.Core.Exceptions
{
    /// <summary>
    ///     Indicates our configuration value is out of range.
    /// </summary>
    public class ConfigurationRangeException : ConfigurationException
    {
        public ConfigurationRangeException()
        {
        }

        public ConfigurationRangeException(string message) : base(message)
        {
        }

        public ConfigurationRangeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConfigurationRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
