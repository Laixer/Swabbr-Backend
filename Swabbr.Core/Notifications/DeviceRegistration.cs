using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Swabbr.Core.Notifications
{
    public class DeviceRegistration
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public PushNotificationPlatform Platform { get; set; }
        public string Handle { get; set; }
        public IList<string> Tags { get; set; }
    }
}
