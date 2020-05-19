using System;
using System.Runtime.Serialization;

namespace Swabbr.Core.Exceptions
{
    public class DeviceNotRegisteredException : Exception
    {
        public DeviceNotRegisteredException()
        {
        }

        public DeviceNotRegisteredException(string message) : base(message)
        {
        }

        public DeviceNotRegisteredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeviceNotRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}