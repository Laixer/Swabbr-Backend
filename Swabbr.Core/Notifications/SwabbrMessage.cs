using Newtonsoft.Json.Linq;
using System;

namespace Swabbr.Core.Notifications
{
    public class SwabbrMessage
    {
        /// <summary>
        /// Title of the notification
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Message/description of the notification
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The name of the protocol
        /// </summary>
        public string Protocol => "swabbr";

        /// <summary>
        /// The version of the protocol
        /// </summary>
        public string ProtocolVersion => "1.0";

        /// <summary>
        /// The client sided action to perform upon clicking the notification alert
        /// </summary>
        public string ClickAction { get; set; }

        /// <summary>
        /// The type of the included data
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// The version of the type of data
        /// </summary>
        public string DataTypeVersion { get; set; }

        /// <summary>
        /// An object of type data_type
        /// </summary>
        public JObject Data { get; set; }

        /// <summary>
        /// Indicates the media type of the content
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Timestamp of when the message was sent
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Contains information about the senders’ operating system, device, etc.
        /// </summary>
        public string UserAgent { get; set; }

        public JObject GetAppleContentJSON()
        {
            // Get base content
            var json = GetContentJSON();

            // Add APNS specific data object
            json.Add(new JProperty("aps",
                new JObject(
                    new JProperty("alert",
                        new JObject(
                            new JProperty("title", Title),
                            new JProperty("body", Body)
                        )
                    )
                )
            ));

            return json;
        }

        public JObject GetFcmContentJSON()
        {
            // Get base content
            var json = GetContentJSON();

            // Add FCM specific data object
            json.Add("notification", new JObject()
            {
               new JProperty("title", Title),
               new JProperty("body", Body)
            });

            return json;
        }

        private JObject GetContentJSON()
        {
            return new JObject(
                new JProperty("protocol", Protocol),
                new JProperty("protocol_version", ProtocolVersion),
                new JProperty("click_action", ClickAction),
                new JProperty("data_type", DataType),
                new JProperty("data_type_version", DataTypeVersion),
                new JProperty("data", Data),
                new JProperty("content_type", ContentType),
                new JProperty("timestamp", TimeStamp),
                new JProperty("user_agent", UserAgent)
            );
        }
    }
}