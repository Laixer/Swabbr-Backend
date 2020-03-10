using System;

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
    }
}