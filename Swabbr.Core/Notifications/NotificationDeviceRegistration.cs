using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Swabbr.Core.Notifications
{
    public class NotificationDeviceRegistration
    {
        public PushNotificationPlatform Platform { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Handle { get; set; }
        public IList<string> Tags { get; set; }
    }
}
