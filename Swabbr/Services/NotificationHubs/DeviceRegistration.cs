using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Swabbr.Api.Services.NotificationHubs
{
    public class DeviceRegistration
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MobilePlatform Platform { get; set; }
        public string Handle { get; set; }
        public IList<string> Tags { get; set; }
    }
}
